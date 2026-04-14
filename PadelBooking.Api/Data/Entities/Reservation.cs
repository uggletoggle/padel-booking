namespace PadelBooking.Api.Data.Entities;

public class Reservation : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid CourtId { get; set; }
    public Guid? FixedReservationId { get; set; }

    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;

    public DateOnly ReservationDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    // Payment fields
    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public bool IsDepositPaid { get; set; }
    public string? PaymentMethod { get; set; }

    public string Status { get; set; } = "Confirmed"; // Pending, Confirmed, Cancelled


    // Navigation
    public Court Court { get; set; } = null!;
    public FixedReservation? FixedReservation { get; set; }
}
