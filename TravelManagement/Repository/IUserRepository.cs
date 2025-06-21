using TravelManagement.Models;
using TravelManagement.Models.DTO;

namespace TravelManagement.Repository
{
    public interface IUserRepository
    {
        Task <User> NewUser(User user);
        public int FindBookingId(int id,DateOnly selectedDate, Models.Status bookingStatus);
        Task<List<Booking>> GetBookingsByUserIdAsync(int userId);

        Task<Dictionary<int, Dictionary<DateOnly, bool>>> GetEmployeeAvailability(int? employeeId = null);

        Task<List<Booking>> FilterUsersBookingsAsync(IQueryable<Booking> query,UserFilterDTO userFilterDTO);

        Task<OvertimeLog> RequestOvertimeAsync(OvertimeRequestDTO overtimeRequestDTO);

        Task<bool> DeleteUser(int id);
    }
}
