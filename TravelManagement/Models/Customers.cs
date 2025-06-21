using System.ComponentModel.DataAnnotations;

namespace TravelManagement.Models
{
    public class Customers
    {
        [Key]
        public int CustomersId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        //make CustomerNumber int to string
        public int CustomerNumber { get; set; }
        public int AlternateNumber { get; set; }
        public DateOnly TravelDate { get; set; }
    }
}
