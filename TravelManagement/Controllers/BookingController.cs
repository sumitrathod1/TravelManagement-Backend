using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelManagement.AppDBContext;
using TravelManagement.Models;
using TravelManagement.Models.DTO;
using TravelManagement.Repository;

namespace TravelManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;

        public BookingController(AppDbContext appDbCotext, IBookingRepository bookingRepository, IUserRepository userRepository)
        {
            _context = appDbCotext;
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBookingById(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound($"Booking with ID {id} not found.");
            }

            return Ok(booking);
        }

        [HttpGet("View-Bookings")]
        public async Task<IActionResult> viewAllbookins()
        {
            var bookingstate = await _bookingRepository.GetAllBookingsWithStatsAsync();

            if (bookingstate == null)
            {
                return NotFound();
            }

            return Ok(bookingstate);
        }
       
        [HttpPost("New-Booking")]
        public async Task<IActionResult> newBooking([FromBody] NewBookiingDTO bookiingDTO)
        {
            Console.WriteLine($"BookingDate received: {bookiingDTO.BookingDate}");
            if (bookiingDTO == null)
            {
                return BadRequest("Please fill the form correctly");
            }
            try
            {
                Booking newBooking = await _bookingRepository.CreateBooking(bookiingDTO);
                return Ok(new { Message = "Booking is Succssfully Added", newBooking });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while adding booking.", Details = ex.Message });
            }
        }

        [HttpPut("Cancel-Booking")]
        public async Task<IActionResult> cancelBooking([FromBody] CancelBookingDTO cancelBookingDTO)
        {
            if (cancelBookingDTO == null)
            {
                return BadRequest();
            }
            int id = cancelBookingDTO.BookingId;

            DateOnly bookingDate = cancelBookingDTO.selectedDate;

            string bookingStatus = cancelBookingDTO.Type;

            Status status = Status.Assigned;
            if (bookingStatus == "Completed")
            {
                status = Status.Completed;
            }
            else if (bookingStatus == "Canceled")
            {
                status = Status.Canceled;
            }
            else if (bookingStatus == "Assigned")
            {
                status = Status.Assigned;
            }
            else if (bookingStatus == "Pending")
            {
                status = Status.Pending;
            }

            int bookingid =  _userRepository.FindBookingId(id, bookingDate, status);
            if (bookingid == -1)
            {
                return BadRequest($"NO booking found for this date {bookingDate}");
            }
            var booking = _bookingRepository.CancelBooking(id);

            if (booking != null)
            {
                return Ok(new { message = "Booking canceled successfully and email sent!" });
            }

            return StatusCode(500, new { message = "Error occurred while canceling the booking" });
        }

        [HttpGet("BookingFilter")]
        public async Task<IActionResult> GetFilteredBookings([FromQuery] BookingFilterDTO filterDTO)
        {
            var filtere = _context.Bookings.AsQueryable();
            var filteredBookings = await _bookingRepository.FilterBookingsAsync(filtere,filterDTO);

            return Ok(filteredBookings);
        }
    }
}
