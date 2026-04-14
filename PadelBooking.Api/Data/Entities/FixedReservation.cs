namespace PadelBooking.Api.Data.Entities;

public class FixedReservation : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid CourtId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;

    public int DayOfWeek { get; set; } // 0 = Sunday, 1 = Monday, ..., 6 = Saturday
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public Court Court { get; set; } = null!;
    public ICollection<Reservation> Reservations { get; set; } = [];
}
