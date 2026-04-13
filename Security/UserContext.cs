using System.Security.Claims;

namespace PadelBooking.Api.Security;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public string? CurrentUserId => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
