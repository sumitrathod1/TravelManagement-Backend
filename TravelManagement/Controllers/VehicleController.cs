using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TravelManagement.AppDBContext;
using TravelManagement.Models;
using TravelManagement.Models.DTO;
using TravelManagement.Repository;

namespace TravelManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IVehicleRepository _vehicleRepository;
        public VehicleController(AppDbContext appDbCotext,IVehicleRepository vehicleRepository  ) 
        {
            _appDbContext = appDbCotext;
            _vehicleRepository = vehicleRepository;
        }

        #region Vehicle

        [HttpGet("GetAllVehicles")]
        

        public async Task<IActionResult> GetallVehicles() 
        {
            var vehicles=await _appDbContext.Vehicles.ToListAsync();
            return Ok(vehicles);
        }

        [HttpGet("GetVehicle")]
        public async Task<ActionResult<Vehicle>> GetVehicle(int id)
        {
            var vehicle = await _appDbContext.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound($"Vehicle with ID {id} not found.");
            }
            return Ok(vehicle);
        }

       // [HttpPost("AddVehicle")]
        [HttpPost("[Action]")]
        public async Task<IActionResult> AddVehcle([FromBody] Vehicle vehicle) 
        {
            if (vehicle == null) {
                return BadRequest("Data can't be null");
            }
            var vechicleCheck = await _appDbContext.Vehicles.AnyAsync( x =>x.VehicleNumber==vehicle.VehicleNumber);
            if (vechicleCheck)
            {
                return BadRequest("Vehicle Is alredy registerd");
            }

            var objVehicle = await _vehicleRepository.AddVehcle(vehicle);
            return Ok(objVehicle);
        }
        [HttpPut("UpdateVehicle")]
        
        public async Task<IActionResult> UpdateVechicle([FromBody] Vehicle vehicle)
        {
            if (vehicle == null)
            {
                return BadRequest("Vehicle not found");
            }

            var updatedVehicle = await _vehicleRepository.UpdateVechicle(vehicle);
            if (updatedVehicle != null)
            {
                return Ok(new { Message = "Vehicle is Updated", updatedVehicle });
            }
            else
            {
                return NotFound("Vehicle not found");
            }
        }

        [HttpGet("vehicle-availability")]
        public async Task<IActionResult> GetVehicleAvailability(int? vehicleId = null)
        {
            if(vehicleId != null)
            {
                var avehicleCheck = await _appDbContext.Vehicles.FindAsync(vehicleId);
                if (avehicleCheck == null)
                  return  BadRequest("Vehicle is not found");
            }
            var availability = await _vehicleRepository.GetVehicleAvailability(vehicleId);

            if (availability == null || !availability.Any())
            {
                return NotFound("No availability found.");
            }

            return Ok(availability);
        }

        #endregion

        #region Vehicle Expense

        [HttpPost("AddVehicleExpence")]
        public async Task<IActionResult> AddVehicleExpence(AddVehicleExpenceDTO addVehicleExpenceDTO)
        {
            if (addVehicleExpenceDTO == null)
                return NotFound();
            var addnewVehicle = await _vehicleRepository.addexpense(addVehicleExpenceDTO);
           return Ok(addnewVehicle);
        }

        [HttpGet("GetAllexpence")]
        public async Task<IActionResult> getAllexpence()
        {
            return Ok(await _appDbContext.vehicleExpences.ToListAsync());
        }

        [HttpGet("GetExpenceBybId")]
        public async Task<IActionResult> GetExpenceBybId(string vehicleNumber)
        {
            bool exists =await _appDbContext.vehicleExpences.AnyAsync(x => x.Vehicle.VehicleNumber == vehicleNumber);
            if (!exists)
                return NotFound();
            var getbyid=await _vehicleRepository.getByidExpence(vehicleNumber);

            return Ok(getbyid);
        }
        #endregion

        #region Document
        [HttpPost("AddDocumentDetails")]
        public async Task<IActionResult> AddDocument([FromBody] Documents documents)
        {
            if (documents == null)
            {
                return BadRequest("Document data is missing");
            }
            bool exists = await _appDbContext.Documents.AnyAsync(x => x.VehicleID == documents.VehicleID);

            if (exists)
            {
                return BadRequest("Vehicle with document already exists. Please update instead.");
            }

            await _appDbContext.AddAsync(documents);
            await _appDbContext.SaveChangesAsync();

            return Ok(documents);
        }

        [HttpGet("GetAlldocuments")]
        public async Task<IActionResult> GetAllDocuments()
        {
            var allDocuments = await _appDbContext.Documents.Include(v => v.Vehicle).ToListAsync();
            return Ok(allDocuments);
        }

        [HttpPut("UpdateDocument")]
        public async Task<IActionResult> UpdateDocument([FromBody] Documents documents)
        {
            if (documents == null)
            {
                return BadRequest("Document data is missing");
            }
            var upDocument = await _appDbContext.Documents.FirstOrDefaultAsync(x => x.VehicleID == documents.VehicleID);
            if (upDocument == null)
                return BadRequest("Document is not found");

            upDocument.Title = documents.Title;
            upDocument.Description = documents.Description;
            upDocument.ExpiryDate = documents.ExpiryDate;

            await _appDbContext.SaveChangesAsync();
            return Ok(new { Message = "document is updated", documents });
        }

        [HttpGet("GetDocumentById")]
        public async Task<IActionResult> GetDocumentById(int id)
        {
            var document = await _appDbContext.Documents.Include(v => v.Vehicle).FirstOrDefaultAsync(d => d.DocumentID == id);
            if (document == null)
                return NotFound();

            return Ok(document);
        }
        #endregion


        [HttpPost("AddVechicleMaintenance")]
        public async Task<IActionResult> AddVechicleMaintenance(VechicleMaintenanceDTO vechicleMaintenanceDTO)
        {
            var addnewMaintainance = await _vehicleRepository.AddNewVechicleMaintenance(vechicleMaintenanceDTO);
            if (addnewMaintainance == null)
                return NotFound("Vehicle not found or invalid data");

            return Ok(addnewMaintainance);
        }
    }
}
