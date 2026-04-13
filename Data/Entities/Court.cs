namespace PadelBooking.Api.Data.Entities;

public class Court : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SurfaceType { get; set; }
    public bool IsCovered { get; set; }

    // 3D layout fields for frontend planner
    public double? LayoutPositionX { get; set; }
    public double? LayoutPositionY { get; set; }
    public double? LayoutRotationZ { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public Club Club { get; set; } = null!;
    public ICollection<Reservation> Reservations { get; set; } = [];
    public ICollection<FixedReservation> FixedReservations { get; set; } = [];
}
