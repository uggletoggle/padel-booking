namespace PadelBooking.Api.DTOs.FixedReservations;

public record CreateFixedReservationRequest(
    Guid CourtId,
    string CustomerName,
    string CustomerPhone,
    int DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    DateOnly StartDate,
    DateOnly? EndDate
);

public record UpdateFixedReservationRequest(
    string CustomerName,
    string CustomerPhone,
    int DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsActive
);

public record FixedReservationResponse(
    Guid Id,
    Guid CourtId,
    string CustomerName,
    string CustomerPhone,
    int DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsActive,
    DateTime CreatedAt
);
