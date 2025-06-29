namespace TravelManagement.Models.DTO
{
    public class AgentDashboardDTO
    {
        public int AgentId { get; set; }
        public string Name { get; set; }
        public TravelAgentType type { get; set; }
        public int BookingCount { get; set; }
        public decimal Earned { get; set; }
        public decimal Pending { get; set; }
    }
}
