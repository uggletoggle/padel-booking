using PadelBooking.Api.Data.Enums;

namespace PadelBooking.Api.Data.Entities;

public class User : AuditableEntity
{
    public Guid Id { get; set; }
    
    public string ExternalAuthId { get; set; } = string.Empty;
    
    public UserRole Role { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    // Optional relationship for Owner/Attendant
    public Guid? ClubId { get; set; }
    public Club? Club { get; set; }
}
