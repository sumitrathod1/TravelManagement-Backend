using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelManagement.AppDBContext;
using TravelManagement.Helper;
using TravelManagement.Models;
using TravelManagement.Models.DTO;
using TravelManagement.Repository;

namespace TravelManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _appDbCotext;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        public UserController(AppDbContext appDbCotext, IUserRepository userRepository, IConfiguration config)
        {
            _appDbCotext = appDbCotext;
            _userRepository = userRepository;
            _config = config;
        }

        
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthDTO authDTO)
        {
            var user = await _appDbCotext.Users.FirstOrDefaultAsync(x => x.UserName == authDTO.userName);
            if (user == null || !PasswordHasher.VerifyPassword(authDTO.Password, user.Password))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = new JwtService(_config).GenerateToken(user);

            return Ok(new { Token = token, Message = "Login Success!" });
        }

        #region User
        [HttpGet("getall-Users")]
        public async Task<IActionResult> GetEmployes()
        {
            var Users = await _appDbCotext.Users.ToListAsync();
            if (Users == null)
            {
                return BadRequest("Users are not found");
            }
            return Ok(Users);
        }

        [HttpGet("get-user")]
        public async Task<IActionResult> GetEmploye(int id)
        {
            var User = await _appDbCotext.Users.FindAsync(id);
            if (User == null)
            {
                return BadRequest("Users are not found");
            }
            return Ok(User);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Invalid Data.");
            }
            var users = await _userRepository.NewUser(user);
            return Ok(new { Message = "User successfully registered", User = users });
        }

        
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            if (changePasswordDTO == null)
                return BadRequest();

            string oldPasswordINPUT = changePasswordDTO.OldPassword;
            string newPassword = changePasswordDTO.NewPassword;
            int id = changePasswordDTO.UserId;

            var user = await _appDbCotext.Users.FirstOrDefaultAsync(x => x.userId == id);
            if (user == null)
                return NotFound(new { message = "User Not Found!" });
            string oldPasswordSTORED = user.Password;
            
            if (!PasswordHasher.VerifyPassword(oldPasswordINPUT, oldPasswordSTORED))
            {
                return BadRequest(new { Message = "Password is Incorrect" });
            }

            user.Password = PasswordHasher.HashPassword(newPassword);

            // Update user entity 
            _appDbCotext.Users.Update(user);
            await _appDbCotext.SaveChangesAsync();

            return Ok(new
            {
                StatusCode = 200,
                Message = "Password reset successfully"
            });
        }

        //Soft delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {

            bool user = await _userRepository.DeleteUser(id);

            if (user)
                return Ok("User Successfully Deleted and Deactivated");
            else
                return NotFound($"User  with ID {id} not found.");
        }
        //-------------------------------------------------

        //Get the bookings of perticular user
        [HttpGet("ViewBookings")]
        public async Task<IActionResult> EmpBookings(int id)
        {
            // Check if user exists
            var bookings = await _userRepository.GetBookingsByUserIdAsync(id);
            if (bookings.Count == 0)
            {
                return new NoContentResult();
            }

            return new OkObjectResult(bookings);
        }

        [HttpGet("employee-availability")]
        public async Task<IActionResult> GetEmployeeAvailability(int? employeeId = null)
        {
            if (employeeId != null)
            {
                var checkUser = await _appDbCotext.Users.FindAsync(employeeId);
                if (checkUser == null)
                    return BadRequest("User not fond");
            }
            
            var availability = await _userRepository.GetEmployeeAvailability(employeeId);

            if (availability == null || !availability.Any())
            {
                return NotFound("No availability found.");
            }

            return Ok(availability);
        }

        [HttpGet("User-BookingFilter")]
        public async Task<IActionResult> GetFilteredBookings([FromQuery] UserFilterDTO userfilterDTO)
        {
            var filtere = _appDbCotext.Bookings.AsQueryable();
            var filteredBookings = await _userRepository.FilterUsersBookingsAsync(filtere, userfilterDTO);

            return Ok(filteredBookings);
        }
        #endregion

        #region OvertimeLog
        [HttpPost("RequestOvertime")]
        public async Task<IActionResult> RequestOvertime([FromBody] OvertimeRequestDTO request)
        {
            try
            {
                var overtime = await _userRepository.RequestOvertimeAsync(request);

                return Ok(new
                {
                    Message = "Overtime request submitted successfully.",
                    overtime.OvertimeID,
                    overtime.hours,
                    overtime.Date,
                    overtime.BookingId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        #endregion

    }
}
