using System.ComponentModel.DataAnnotations;

namespace TravelManagement.Models
{
    public class Customers
    {
        [Key]
        public int CustomersId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        //make CustomerNumber int to string
        public string? CustomerNumber { get; set; }
        public string? AlternateNumber { get; set; } = string.Empty;
        public DateOnly TravelDate { get; set; }
    }
}
