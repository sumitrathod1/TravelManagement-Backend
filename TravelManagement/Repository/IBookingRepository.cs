using TravelManagement.Models;
using TravelManagement.Models.DTO;

namespace TravelManagement.Repository
{
    public interface IBookingRepository
    {
        public Booking CancelBooking(int BookingId);
        Task<Booking> CreateBooking(NewBookiingDTO newBookingDTO);
        Task<List<Booking>> GetAllBookingsAsync();
        public Task<object> GetAllBookingsWithStatsAsync();
        Task<List<Booking>> FilterBookingsAsync(IQueryable<Booking> query,
            BookingFilterDTO filterDTO
        );
    }
}
