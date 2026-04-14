namespace PadelBookingAdmin.Models.Users
{
    public enum UserRole
    {
        Admin,
        Owner,
        Attendant
    }

    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public Guid? ClubId { get; set; }
    }

    public class CreateAdminUserRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public Guid? ClubId { get; set; }
    }
}
