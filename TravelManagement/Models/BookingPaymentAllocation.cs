using System.ComponentModel.DataAnnotations;

namespace TravelManagement.Models
{
    public enum PayerType
    {
        Customer,
        Owner,
        Agent
    }   
    public class BookingPaymentAllocation
    {
        [Key]
        public int PaymentAllocationId { get; set; }
        public int BookingId { get; set; }
        public Booking Booking { get; set; }
        public PayerType PayerType { get; set; } // "Customer", "Owner", "Agent"
        public int? CustomerId { get; set; }
        public Customers? Customers { get; set; } 
        public int? TravelAgentId { get; set; } 
        public TravelAgent? TravelAgent { get; set; } 
        public decimal AllocatedAmount { get; set; } // e.g. ₹3000

        // Optional: For UI/report
        public decimal PaidAmount { get; set; } = 0; // Auto-sum from Payment table (or update manually)
    }
}
