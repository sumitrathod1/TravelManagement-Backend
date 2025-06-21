using System.ComponentModel.DataAnnotations;

namespace TravelManagement.Models
{
    public enum TravelAgentType
    {
        Agent,
        TravelOwner
    }
    public class TravelAgent
    {
        [Key]
        public int AgentId { get; set; }
        public string Name { get; set; }
        public TravelAgentType type { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public decimal? CommissionRate { get; set; }
    }
}
