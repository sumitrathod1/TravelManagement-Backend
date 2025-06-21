namespace TravelManagement.Models.DTO
{
    public class OvertimeRequestDTO
    {
            public int UserId { get; set; }
            public decimal Hours { get; set; }
            public string Description { get; set; } = string.Empty;
            public DateTime Date { get; set; }
            public int BookingId { get; set; }
    }
}
