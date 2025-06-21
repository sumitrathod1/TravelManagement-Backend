using System.ComponentModel.DataAnnotations;

namespace TravelManagement.Models
{
    public enum VechileType
    {
        HatchBack,
        Sedan,
        Suv,
        TT17Seater,
        TT20Seater,
        Bus30Seater,
        Bus40Seater,
        Bus60Seater,
        Notspecified
    }
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public string? VehicleNumber { get; set; }
        public VechileType VehicleType { get; set; }
        public DateOnly RegistrationDate { get; set; }
        public double VehicleAge { get; set; }
        public int Seatingcapacity { get; set; }
    }
}
