using PadelBooking.Api.Data.Enums;

namespace PadelBooking.Api.DTOs.Users;

public record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    UserRole Role,
    Guid? ClubId
);
