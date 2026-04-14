namespace PadelBooking.Api.Security;

public interface IUserContext
{
    string? CurrentUserId { get; }
}
