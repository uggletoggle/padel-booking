using PadelBooking.Api.Data.Enums;

namespace PadelBooking.Api.DTOs.Users;

public record CreateAdminUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    UserRole Role,
    Guid? ClubId
);
