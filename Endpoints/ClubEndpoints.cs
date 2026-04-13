using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PadelBooking.Api.Data.Entities;
using PadelBooking.Api.Data.Repositories;
using PadelBooking.Api.DTOs.Clubs;

namespace PadelBooking.Api.Endpoints;

public static class ClubEndpoints
{
    public static RouteGroupBuilder MapClubEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllClubs);
        group.MapGet("/{id:guid}", GetClubById);
        group.MapPost("/", CreateClub).RequireAuthorization("RequireAdminRole");
        group.MapPut("/{id:guid}", UpdateClub).RequireAuthorization("RequireAdminRole");
        group.MapDelete("/{id:guid}", DeleteClub).RequireAuthorization("RequireAdminRole");

        return group;
    }

    private static async Task<Ok<IEnumerable<ClubResponse>>> GetAllClubs(
        IRepository<Club> repository,
        CancellationToken cancellationToken)
    {
        var clubs = await repository.GetAllAsync(cancellationToken);
        return TypedResults.Ok(clubs.Adapt<IEnumerable<ClubResponse>>());
    }

    private static async Task<Results<Ok<ClubResponse>, NotFound>> GetClubById(
        Guid id,
        IRepository<Club> repository,
        CancellationToken cancellationToken)
    {
        var club = await repository.GetByIdAsync(id, cancellationToken);
        return club is null ? TypedResults.NotFound() : TypedResults.Ok(club.Adapt<ClubResponse>());
    }

    private static async Task<Results<Created<ClubResponse>, ValidationProblem>> CreateClub(
        [FromBody] CreateClubRequest request,
        IRepository<Club> repository,
        IValidator<CreateClubRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var club = request.Adapt<Club>();
        await repository.AddAsync(club, cancellationToken);
        
        var response = club.Adapt<ClubResponse>();
        return TypedResults.Created($"/api/clubs/{response.Id}", response);
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateClub(
        Guid id,
        [FromBody] UpdateClubRequest request,
        IRepository<Club> repository,
        IValidator<UpdateClubRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var club = await repository.GetByIdAsync(id, cancellationToken);
        if (club is null)
        {
            return TypedResults.NotFound();
        }

        request.Adapt(club);
        await repository.UpdateAsync(club, cancellationToken);

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteClub(
        Guid id,
        IRepository<Club> repository,
        CancellationToken cancellationToken)
    {
        var club = await repository.GetByIdAsync(id, cancellationToken);
        if (club is null)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(club, cancellationToken);
        return TypedResults.NoContent();
    }
}
