using System.ComponentModel.DataAnnotations;

namespace TravelManagement.Models
{
    public class Documents
    {
        [Key]
        public int DocumentID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime ExpiryDate  { get; set; }
        public int VehicleID { get; set; }
        public Vehicle? Vehicle { get; set; }
    }
}
