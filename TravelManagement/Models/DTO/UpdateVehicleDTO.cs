namespace TravelManagement.Models.DTO
{
    public class UpdateVehicleDTO
    {
        public int VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public string? VehicleNumber { get; set; }
        public string VehicleType { get; set; }
        public DateOnly RegistrationDate { get; set; }
        public int Seatingcapacity { get; set; }
    }
}
