using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelManagement.AppDBContext;
using TravelManagement.Models.DTO;
using TravelManagement.Repository;

namespace TravelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class InquiryController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly BookingRepository _bookingRepo;

        public InquiryController(AppDbContext db, BookingRepository bookingRepo)
        {
            _db = db;
            _bookingRepo = bookingRepo;
        }
        [HttpGet("GetAllEnqueries")]
        public async Task<IActionResult> GetAllEnqueries()
        {
            var inquiries = await _db.EmailInquiries
                .OrderByDescending(e => e.Id)
                .ToListAsync();
            return Ok(inquiries);
        }

        [HttpPost("confirm/{id}")]
        public async Task<IActionResult> Confirm(int id)
        {
            var inquiry = await _db.EmailInquiries.FindAsync(id);
            if (inquiry == null) return NotFound();
            if (inquiry.IsConfirmed || inquiry.IsRejected) return BadRequest("Already processed");
            var dto = new NewBookiingDTO
            {
                CustomerName = inquiry.CustomerName,
                CustomerNumber = inquiry.CustomerNumber,
                From = inquiry.From,
                To = inquiry.To,
                Pax = inquiry.Pax,
                BookingDate = inquiry.TravelDate ?? DateOnly.FromDateTime(DateTime.Today),
                BookingTime = TimeOnly.FromDateTime(DateTime.Now),
                VehicleId = await GetVehicleIdByNameAsync(inquiry.VehicleName, _db),
                BookingType = "Notspecified",
                BookingStatus = "Pending",
                Amount = 0,
                Payment = "Admin",
                UserId = 2
            };
            var booking = await _bookingRepo.CreateBooking(dto);
            inquiry.IsConfirmed = true;
            _db.EmailInquiries.Update(inquiry);
            await _db.SaveChangesAsync();
            return Ok(booking);
        }

        [HttpPost("reject/{id}")]
        public async Task<IActionResult> Reject(int id)
        {
            var inquiry = await _db.EmailInquiries.FindAsync(id);
            if (inquiry == null) return NotFound();
            inquiry.IsRejected = true;
            _db.EmailInquiries.Update(inquiry);
            await _db.SaveChangesAsync();
            return Ok("Inquiry rejected");
        }
        private async Task<int> GetVehicleIdByNameAsync(string? vehicleName, AppDbContext dbContext)
        {
            if (string.IsNullOrWhiteSpace(vehicleName)) return 1;
            var vehicle = await dbContext.Vehicles.FirstOrDefaultAsync(v => v.VehicleName == vehicleName);
            return vehicle?.VehicleId ?? 1;
        }
    }
}
