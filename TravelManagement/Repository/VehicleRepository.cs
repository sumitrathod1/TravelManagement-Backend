using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TravelManagement.AppDBContext;
using TravelManagement.Models;
using TravelManagement.Models.DTO;

namespace TravelManagement.Repository
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AppDbContext _appDbCotext; 
        public VehicleRepository(AppDbContext appDbCotext) 
        {
            _appDbCotext = appDbCotext;
        }
        public async Task<Vehicle> AddVehcle(Vehicle vehicle)
        {
            DateOnly rdate = vehicle.RegistrationDate;
            Vehicle newVehicle = new Vehicle
            {
                VehicleName = vehicle.VehicleName,
                VehicleNumber = vehicle.VehicleNumber,
                VehicleType = vehicle.VehicleType,
                RegistrationDate = rdate,
                Seatingcapacity = vehicle.Seatingcapacity,
                VehicleAge = Helper.Claculations.CalculateAge(rdate, DateTime.Now),
            };

            await _appDbCotext.AddAsync(newVehicle);
            await _appDbCotext.SaveChangesAsync();

            return newVehicle;
        }

        public async Task<Vehicle> UpdateVechicle(Vehicle vehicle)
        {
            var updateVehicle = await _appDbCotext.Vehicles.FirstOrDefaultAsync(x => x.VehicleId == vehicle.VehicleId);
            if (updateVehicle != null)
            {
                updateVehicle.VehicleNumber = vehicle.VehicleNumber;
                updateVehicle.VehicleName = vehicle.VehicleName;
                updateVehicle.RegistrationDate = vehicle.RegistrationDate;
                updateVehicle.VehicleType = vehicle.VehicleType;
                updateVehicle.Seatingcapacity = vehicle.Seatingcapacity;
                await _appDbCotext.SaveChangesAsync();
            }
            return vehicle;
        }

        public async Task<Dictionary<int, Dictionary<DateOnly, bool>>> GetVehicleAvailability(int? VehicleId = null)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var endDate = today.AddDays(20);

            var vehicleAvailability = new Dictionary<int, Dictionary<DateOnly, bool>>();

            // If VehicleId is provided, fetch only that vehicle
            if (VehicleId.HasValue)
            {
                var vehicle = await _appDbCotext.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == VehicleId.Value);
                if (vehicle != null)
                {
                    var availability = new Dictionary<DateOnly, bool>();
                    for (var date = today; date <= endDate; date = date.AddDays(1))
                    {
                        availability[date] = true; // Set all days as available initially
                    }
                    vehicleAvailability[vehicle.VehicleId] = availability;
                }
            }
            else
            {
                // Fetch all vehicles if no VehicleId is provided
                var vehicles = await _appDbCotext.Vehicles.ToListAsync();
                foreach (var vehicle in vehicles)
                {
                    var availability = new Dictionary<DateOnly, bool>();
                    for (var date = today; date <= endDate; date = date.AddDays(1))
                    {
                        availability[date] = true; // Set all days as available initially
                    }
                    vehicleAvailability[vehicle.VehicleId] = availability;
                }
            }

            // Fetch bookings and mark unavailable dates
            var query = _appDbCotext.Bookings.AsQueryable();

            if (VehicleId.HasValue)
            {
                query = query.Where(b => b.VehicleId == VehicleId.Value); // Filter by specific vehicle
            }

            var bookings = await query
                .Where(b => b.travelDate >= today && b.travelDate <= endDate &&
                            b.Status != Status.Canceled) // Ignore canceled bookings
                .ToListAsync();

            foreach (var booking in bookings)
            {
                    var vehicleId = booking.VehicleId;
                    if (vehicleAvailability.ContainsKey(vehicleId) && vehicleAvailability[vehicleId].ContainsKey(booking.travelDate))
                    {
                        vehicleAvailability[vehicleId][booking.travelDate] = false; // Mark as unavailable
                    }
            }

            // Return availability for the requested vehicle or all vehicles
            if (VehicleId.HasValue)
            {
                return new Dictionary<int, Dictionary<DateOnly, bool>>
                {
                    { VehicleId.Value, vehicleAvailability[VehicleId.Value] }
                };
            }

            return vehicleAvailability; // Return availability for all vehicles
        }

        public async Task<VehicleExpence> addexpense(AddVehicleExpenceDTO addVehicleExpenceDTO)
        {
            Category category=Category.Repair;
            switch (addVehicleExpenceDTO.CategoryType.ToString())
            {
                case "Accident":
                    category = Category.Accident; break;
                case "Towing":
                    category=Category.Towing; break;
                case "DocumentRenew":
                    category= Category.DocumentRenew; break;
            }
            var newVehicelExpense = new VehicleExpence
            {
                ExpenseDate = DateTime.Now,
                Amount = addVehicleExpenceDTO.Amount,
                CategoryType = category,
                VehicleID = addVehicleExpenceDTO.VehicleID
            };
            await _appDbCotext.AddAsync(newVehicelExpense);
            await _appDbCotext.SaveChangesAsync();

            return newVehicelExpense;
        }

        public async Task<VehicleExpence> getByidExpence(string vhNumber)
        {
            var byVehicleid = await _appDbCotext.vehicleExpences.
                                    Include(x => x.Vehicle).
                                    FirstOrDefaultAsync(y=>y.Vehicle.VehicleNumber==vhNumber);
            
            return byVehicleid;
        }

        public async Task<VehicleMaintenanceShedule> AddNewVechicleMaintenance(VechicleMaintenanceDTO vechicleMaintenanceDTO)
        {
            var exists = await _appDbCotext.Vehicles.FindAsync(vechicleMaintenanceDTO.VehicleID);
            if (exists == null)
                return null;

            MaintenanceType mType = MaintenanceType.Service;
            switch (vechicleMaintenanceDTO.maintenanceType)
            {
                case "oilChange":
                    mType = MaintenanceType.oilChange; break;
                case "TireChange":
                    mType = MaintenanceType.TireChange; break;
            }
            VehicleMaintenanceShedule newVhM = new VehicleMaintenanceShedule
            {
                ServieDate = vechicleMaintenanceDTO.ServieDate,
                Nextduedate = vechicleMaintenanceDTO.Nextduedate,
                Description = vechicleMaintenanceDTO.Description,
                cost = vechicleMaintenanceDTO.cost,
                maintenanceType = mType,
                VehicleID = vechicleMaintenanceDTO.VehicleID,
            };
            await _appDbCotext.AddAsync(newVhM);
            await _appDbCotext.SaveChangesAsync();

            return newVhM;
        }

        public async Task<List<VehicleMaintenanceShedule>> GetMaintenanceShedule()
        {
            var maintenace = await _appDbCotext.vehicleMaintenanceShedules
            .Include(b => b.Vehicle)
            .ToListAsync();

            return maintenace;
        }
    }
}
