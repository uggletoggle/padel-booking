namespace PadelBookingAdmin.Models.Clubs
{
    public class CourtResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    
    public class ClubResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string OpenTime { get; set; } = string.Empty;
        public string CloseTime { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<CourtResponse> Courts { get; set; } = new();
    }

    public class CreateClubRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string OpenTime { get; set; } = string.Empty;
        public string CloseTime { get; set; } = string.Empty;
    }

    public class UpdateClubRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string OpenTime { get; set; } = string.Empty;
        public string CloseTime { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
