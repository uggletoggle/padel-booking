namespace PadelBooking.Api.DTOs.Reservations;

public record CreateReservationRequest(
    Guid CourtId,
    Guid? FixedReservationId,
    string CustomerName,
    string CustomerPhone,
    DateOnly ReservationDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    decimal TotalAmount,
    decimal DepositAmount,
    bool IsDepositPaid,
    string? PaymentMethod,
    string Status
);

public record UpdateReservationRequest(
    string CustomerName,
    string CustomerPhone,
    DateOnly ReservationDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    decimal TotalAmount,
    decimal DepositAmount,
    bool IsDepositPaid,
    string? PaymentMethod,
    string Status
);

public record ReservationResponse(
    Guid Id,
    Guid CourtId,
    Guid? FixedReservationId,
    string CustomerName,
    string CustomerPhone,
    DateOnly ReservationDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    decimal TotalAmount,
    decimal DepositAmount,
    bool IsDepositPaid,
    string? PaymentMethod,
    string Status,
    DateTime CreatedAt
);
