namespace PadelBooking.Api.Data.Entities;

public class Club : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Court> Courts { get; set; } = [];
}
