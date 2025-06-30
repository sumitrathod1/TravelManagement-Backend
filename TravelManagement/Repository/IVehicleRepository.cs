using TravelManagement.Models;
using TravelManagement.Models.DTO;

namespace TravelManagement.Repository
{
    public interface IVehicleRepository
    {
        Task<Vehicle> AddVehcle(Vehicle vehicle);
        Task<Vehicle> UpdateVechicle(Vehicle vehicle);

        Task<Dictionary<int, Dictionary<DateOnly, bool>>> GetVehicleAvailability(int? employeeId = null);

        Task<VehicleExpence> addexpense(AddVehicleExpenceDTO addVehicleExpenceDTO);

        Task<VehicleExpence> getByidExpence(string vehicleNumber);

        Task<List<VehicleMaintenanceShedule>> GetMaintenanceShedule();

        Task<VehicleMaintenanceShedule> AddNewVechicleMaintenance(VechicleMaintenanceDTO vechicleMaintenanceDTO);
    }
}
