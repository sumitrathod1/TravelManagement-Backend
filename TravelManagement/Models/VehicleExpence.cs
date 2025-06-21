using System.ComponentModel.DataAnnotations;

namespace TravelManagement.Models
{
    public enum Category 
    {
        Repair,
        Accident,
        Towing,
        DocumentRenew
    }
    public class VehicleExpence
    {
        [Key]
        public int VehicleExpenceId { get; set; }
        public DateTime ExpenseDate { get; set; }
        public decimal Amount { get; set; }
        public Category CategoryType { get; set; }
        public int VehicleID { get; set; }
        public Vehicle? Vehicle { get; set; }
    }
}
