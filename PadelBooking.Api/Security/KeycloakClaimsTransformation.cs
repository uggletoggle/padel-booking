using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using PadelBooking.Api.Data;

namespace PadelBooking.Api.Security;

public class KeycloakClaimsTransformation(IServiceProvider serviceProvider) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Don't transform if already transformed or no user id
        if (principal.HasClaim(c => c.Type == ClaimTypes.Role) || principal.Identity?.IsAuthenticated != true)
        {
            return principal;
        }

        var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return principal;
        }

        // We use IServiceProvider because IClaimsTransformation is typically transient/singleton
        // and DbContext is scoped. We need to create a scope to access it safely.
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var emailClaim = principal.FindFirstValue(ClaimTypes.Email) ?? principal.FindFirstValue("email");

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.ExternalAuthId == userIdClaim);

        // Auto-heal logic: If user isn't found by ID, but we seeded them with a dummy ID, link them by Email instead.
        if (user == null && emailClaim != null)
        {
            user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == emailClaim && u.ExternalAuthId.Contains("keycloak-id"));
            if (user != null)
            {
                // Update the dummy seed ID with the real Keycloak UUID
                user.ExternalAuthId = userIdClaim;
                await dbContext.SaveChangesAsync();
            }
        }

        if (user != null)
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));
            
            // Add ClubId if not null so policies/endpoints can use it
            if (user.ClubId.HasValue)
            {
                identity.AddClaim(new Claim("ClubId", user.ClubId.Value.ToString()));
            }

            principal.AddIdentity(identity);
        }

        return principal;
    }
}
