using System.ComponentModel.DataAnnotations;

namespace TravelManagement.Models.DTO
{
    public class addAgentDTO
    {
        public string Name { get; set; }

        [Phone]
        public string ContactNumber { get; set; }
        public string AgentType { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
