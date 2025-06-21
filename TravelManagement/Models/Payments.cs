using System.ComponentModel.DataAnnotations;

namespace TravelManagement.Models
{
    public class Payments
    {
        [Key]
        public int PaymentId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentMethod { get; set; } // e.g., online, Cash
        public int BookingId { get; set; }
        public Booking Booking { get; set; }
        public int? TravelAgentId { get; set; } // if payment by agent
        public TravelAgent? TravelAgent { get; set; }
        public int? CustomerId { get; set; } // if payment by customer
        public Customers? Customer { get; set; }
    }
}
