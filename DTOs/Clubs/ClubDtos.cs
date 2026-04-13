namespace PadelBooking.Api.DTOs.Clubs;

public record CreateClubRequest(
    string Name,
    string? Address,
    TimeOnly OpenTime,
    TimeOnly CloseTime
);

public record UpdateClubRequest(
    string Name,
    string? Address,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    bool IsActive
);

public record ClubResponse(
    Guid Id,
    string Name,
    string? Address,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    bool IsActive,
    DateTime CreatedAt
);
