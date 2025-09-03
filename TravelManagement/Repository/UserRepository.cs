using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TravelManagement.AppDBContext;
using TravelManagement.Models;
using TravelManagement.Models.DTO;

namespace TravelManagement.Repository
{
    public class UserRepository: IUserRepository 
    {
        private readonly AppDbContext _appDbCotext;


        public UserRepository(AppDbContext appDbCotext) 
        {
            _appDbCotext = appDbCotext;
        }

        // Instance method to fetch bookings
        public async Task<List<Booking>> GetBookingsByUserIdAsync(int id)
        {
            // Get the bookings for the user
            var bookings = await _appDbCotext.Bookings
                .Where(x => x.Userid == id) 
                .Include(x => x.Customer)     
                .Include(x => x.Vehicle)
                .ToListAsync();
            return bookings;
        }

        public async Task<User> NewUser(User user)
        {
            if (!user.EmployeeDOB.HasValue)
            {
                throw new InvalidOperationException("EmployeeDOB is required.");
            }
            DateOnly dateTime = (DateOnly)user.EmployeeDOB;
            user.Password=PasswordHasher.HashPassword(user.Password);
            user.EmployeAge = (int)Helper.Claculations.CalculateAge(dateTime,DateTime.Now);
            await _appDbCotext.AddAsync(user);
            await _appDbCotext.SaveChangesAsync();
            await addSalary(user.Salary,user.userId);
            return user;
        }   

        public async Task<bool> DeleteUser(int id)
        {
            var user=await _appDbCotext.Users.FirstOrDefaultAsync(x=>x.userId==id);
            if (user == null)
                return false;

            user.Status=false;
            await _appDbCotext.SaveChangesAsync();
            return true;
        }

        public int FindBookingId(int id, DateOnly selectedDate, Models.Status bookingStatus)
        {
            var user = _appDbCotext.Bookings.Where(x => x.Userid == id && x.Status == bookingStatus && x.travelDate == selectedDate).FirstOrDefault();
            if(user == null)
            {
                return -1;
            }
            int bookingid= (int)user.Userid;
            return bookingid;
        }

        public async Task<Dictionary<int, Dictionary<DateOnly, bool>>> GetEmployeeAvailability(int? employeeId = null)
        {
            var today = DateOnly.FromDateTime(DateTime.Today); 
            var endDate = today.AddDays(20); 

            
            var allEmployees = await _appDbCotext.Users.ToListAsync();

            // Step 2: Initialize availability for all employees
            var employeeAvailability = new Dictionary<int, Dictionary<DateOnly, bool>>();

            // Loop through all employees and initialize their availability to true (available)
            foreach (var employee in allEmployees)
            {
                var availability = new Dictionary<DateOnly, bool>();
                for (var date = today; date <= endDate; date = date.AddDays(1))
                {
                    availability[date] = true; // Set all days as available initially
                }
                employeeAvailability[employee.userId] = availability; // Employee's availability initialized
            }

            // Step 3: Fetch bookings for employees in the next 20 days (for those with bookings)
            var query = _appDbCotext.Bookings.AsQueryable();

            // If employeeId is provided, filter bookings for that employee only
            if (employeeId.HasValue)
            {
                query = query.Where(b => b.Userid == employeeId.Value); // Only for the specified employee
            }

            // Fetch bookings for all employees or for the specified employee
            var bookings = await query
                .Where(b => b.travelDate >= today && b.travelDate <= endDate &&
                            b.Status != Status.Canceled) // Ignore canceled bookings
                .ToListAsync();

            // Step 4: Mark days with bookings as unavailable for each employee
            foreach (var booking in bookings)
            {
                if (employeeAvailability.ContainsKey(booking.Userid ?? 0) &&
                    employeeAvailability[booking.Userid ?? 0].ContainsKey(booking.travelDate))
                {
                    employeeAvailability[booking.Userid ?? 0][booking.travelDate] = false; // Mark as unavailable
                }
            }

            // Step 5: Return the availability for the requested employee or all employees
            if (employeeId.HasValue)
            {
                return new Dictionary<int, Dictionary<DateOnly, bool>>
                {
                    { employeeId.Value, employeeAvailability[employeeId.Value] }
                };
            }
            return employeeAvailability; // Return availability for all employees
        }
        
         
        public async Task<List<Booking>> FilterUsersBookingsAsync(IQueryable<Booking> query, UserFilterDTO filterDTO)
        {
            query = query.Where(b => b.Userid == filterDTO.userId);

            if (filterDTO.PerticularDate.HasValue)
            {
                query = query.Where(b => b.travelDate == filterDTO.PerticularDate.Value);
            }
            if (filterDTO.StartDate.HasValue)
            {
                query = query.Where(b => b.travelDate >= filterDTO.StartDate.Value);
            }

            if (filterDTO.EndDate.HasValue)
            {
                query = query.Where(b => b.travelDate <= filterDTO.EndDate.Value);
            }

            if (filterDTO.Status.HasValue)
            {
                query = query.Where(b => b.Status == filterDTO.Status.Value);
            }

            if (filterDTO.BookingType.HasValue)
            {
                query = query.Where(b => b.BookingType == filterDTO.BookingType.Value);
            }

            if (filterDTO.TravelTime.HasValue)
            {
                query = query.Where(b => b.Traveltime == filterDTO.TravelTime.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Salary> addSalary(decimal baseSalary, int userId)
        {
            var currentDate = DateTime.Now;

            Salary salary = new Salary
            {
                BaseSalay = baseSalary,
                Deduction = 0, // default
                Overtimepay = 0, // default
                NetSalaey = baseSalary, // initially no deductions or extras
                Month = currentDate.Month,
                Year = currentDate.Year,
                userID = userId
            };

            await _appDbCotext.salaries.AddAsync(salary);
            await _appDbCotext.SaveChangesAsync();

            return salary;
        }
        public async Task<OvertimeLog> RequestOvertimeAsync(OvertimeRequestDTO request)
        {
            if (request.Hours <= 0)
                throw new ArgumentException("Minimum overtime must be 1 hour");

            var user = await _appDbCotext.Users.FindAsync(request.UserId);
            if (user == null) throw new Exception("User not found");

            var booking = await _appDbCotext.Bookings.FindAsync(request.BookingId);
            if (booking == null) throw new Exception("Booking not found");

            var overtime = new OvertimeLog
            {
                userId = request.UserId,
                hours = request.Hours,
                Description = request.Description,
                Date = request.Date,
                BookingId = request.BookingId,
                IsApproved = false
            };

            await _appDbCotext.overtimeLogs.AddAsync(overtime);
            await _appDbCotext.SaveChangesAsync();

            return overtime;
        }
    }
}
