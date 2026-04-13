using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using PadelBooking.Api.Data;
using PadelBooking.Api.Data.Entities;
using PadelBooking.Api.DTOs.Users;
using PadelBooking.Api.Services;

namespace PadelBooking.Api.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateUser)
            .RequireAuthorization("RequireAdminRole")
            .Produces<User>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", GetUsers)
            .RequireAuthorization("RequireAdminRole")
            .Produces<List<UserResponse>>(StatusCodes.Status200OK);

        return group;
    }

    private static async Task<IResult> GetUsers(AppDbContext dbContext)
    {
        var users = await dbContext.Users
            .AsNoTracking()
            .ProjectToType<UserResponse>()
            .ToListAsync();
            
        return Results.Ok(users);
    }

    private static async Task<IResult> CreateUser(
        CreateAdminUserRequest request,
        IValidator<CreateAdminUserRequest> validator,
        KeycloakAdminService keycloakAdmin,
        AppDbContext dbContext)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        // Validate club exists if provided
        if (request.ClubId.HasValue)
        {
            var clubExists = await dbContext.Clubs.AnyAsync(c => c.Id == request.ClubId.Value);
            if (!clubExists)
            {
                return Results.BadRequest(new { error = "The specified ClubId does not exist." });
            }
        }

        // Validate user email doesn't exist locally just to be safe
        var localExists = await dbContext.Users.AnyAsync(u => u.Email == request.Email);
        if (localExists)
        {
             return Results.Conflict(new { error = "User with this email already exists." });
        }

        string externalAuthId;
        
        // 1. Transactional constraint: Create in Keycloak. 
        // If this fails, no DB record is created.
        try
        {
            externalAuthId = await keycloakAdmin.CreateUserAsync(
                request.Email, 
                request.FirstName, 
                request.LastName, 
                request.Password
            );
        }
        catch (InvalidOperationException ex)
        {
             return Results.Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // For production you'd use ILogger, returning 500 here
            return Results.Problem(detail: ex.Message, title: "Keycloak Provisioning Failed");
        }

        // 2. Create in Local Database
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            ExternalAuthId = externalAuthId,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role,
            ClubId = request.ClubId
        };

        dbContext.Users.Add(newUser);
        
        try 
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Compensation Transaction: Delete from Keycloak if DB save fails
            try 
            {
                await keycloakAdmin.DeleteUserAsync(externalAuthId);
            }
            catch (Exception compensationEx)
            {
                return Results.Problem(
                    detail: $"FATAL: Database save failed ({ex.Message}), and reverting Keycloak creation ALSO failed ({compensationEx.Message}). User {externalAuthId} is orphaned in Keycloak.",
                    statusCode: 500
                );
            }

            return Results.Problem(
                detail: $"Failed to save user to database (Role might be invalid, or DB is down). The user creation was completely rolled back in Keycloak. Detail: {ex.Message}",
                title: "Transaction Rolled Back"
            );
        }

        return Results.Created($"/api/users/{newUser.Id}", newUser);
    }
}
