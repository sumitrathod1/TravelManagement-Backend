using System.ComponentModel.DataAnnotations;

namespace TravelManagement.Models
{
    public class ExternalEmployeeCashCollection
    {
        [Key]
        public int ExtEmpID { get; set; }
        public decimal AmountOwed { get;  set; }
        public decimal AmountCollexted { get; set; }
        public bool Ispaid { get; set; }
        public int Bookingid { get; set; }
        public Booking? Booking { get; set; }
        public int ExternalEmployeId { get; set; }
        public ExternalEmployee? ExternalEmployee { get; set; }

    }
}
