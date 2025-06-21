using System.ComponentModel.DataAnnotations;

namespace TravelManagement.Models
{
    public class ExternalEmployee
    {
        [Key]
        public int externalEmployeeID { get; set; }
        public string? externalEmployeeName { get; set; }
        public int externalEmployeeNumber { get; set; }

    }
}
