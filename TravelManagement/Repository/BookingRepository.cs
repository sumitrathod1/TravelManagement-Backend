using TravelManagement.AppDBContext;
using TravelManagement.Models;
using Microsoft.EntityFrameworkCore;
using TravelManagement.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TravelManagement.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _appDbCotext;
        public BookingRepository(AppDbContext appDbCotext) 
        {
            _appDbCotext = appDbCotext;
        }

        public async Task<object> GetAllBookingsWithStatsAsync()
        {
            var bookings = await _appDbCotext.Bookings
                .AsNoTracking()
                .Include(b => b.user)
                .Include(b => b.Customer)
                .Include(b => b.Vehicle)
                .Select(b => new
                {
                    b.BookingId,
                    b.travelDate,
                    b.Traveltime,
                    b.BookingType,
                    b.From,
                    b.To,
                    b.Amount,
                    b.Status,
                    b.Pax,
                    Customer = new
                    {
                        b.Customer.CustomerName,
                        b.Customer.CustomerNumber
                    },
                    Vehicle = new
                    {
                        b.Vehicle.VehicleName
                    },
                    User = new
                    {
                        b.user.EmployeeName
                    }
                })
                .ToListAsync();

            // Revenue Calculations
            var today = DateOnly.FromDateTime(DateTime.Today);
            var currentMonth = today.Month;
            var currentYear = today.Year;
            var sevenDaysAgo = today.AddDays(-6);

            var totalToday = bookings
                .Where(b => b.travelDate == today)
                .Sum(b => b.Amount);

            var totalWeek = bookings
                .Where(b => b.travelDate >= sevenDaysAgo && b.travelDate <= today)
                .Sum(b => b.Amount);

            var totalMonth = bookings
                .Where(b => b.travelDate.Month == currentMonth && b.travelDate.Year == currentYear)
                .Sum(b => b.Amount);

            var totalYear = bookings
                .Where(b => b.travelDate.Year == currentYear)
                .Sum(b => b.Amount);

            var totalRevenue = bookings.Sum(b => b.Amount);

            return new
            {
                bookings,
                revenueStats = new
                {
                    today = totalToday,
                    week = totalWeek,
                    month = totalMonth,
                    year = totalYear,
                    total = totalRevenue
                }
            };
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _appDbCotext.Bookings
                .Include(b => b.user)
                .Include(c => c.Customer)
                .OrderByDescending(b=>b.BookingId)
                .Include(v => v.Vehicle).Take(100).ToListAsync();
        }
        public Booking CancelBooking(int BookingId)
        {
            Booking? Booking = _appDbCotext.Bookings.Find(BookingId);
            Booking.Status = Status.Canceled;
            Update(Booking);
            return Booking;
        }
        public Booking Update(Booking booking)
        {
            var Booking = _appDbCotext.Bookings.Attach(booking);
            Booking.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _appDbCotext.SaveChanges();
            return booking;
        }
        public async Task<Booking> CreateBooking(NewBookiingDTO newBookiingDTO)
        {
            // 1. Map enums
            BookingType bookingType = Enum.TryParse<BookingType>(newBookiingDTO.BookingType, out var bType)
                ? bType : BookingType.Notspecified;

            Status bookingStatus = Enum.TryParse<Status>(newBookiingDTO.BookingStatus, out var status)
                ? status : Status.Pending;

            Payment paymentSource = newBookiingDTO.Payment == "ExternalEmployee"
                ? Payment.ExternalEmployee : Payment.Admin;

            // 2. Check for existing booking
            var existingBooking = await _appDbCotext.Bookings
                .FirstOrDefaultAsync(x => x.BookingId == newBookiingDTO.BookingId);

            // 3. Prepare TravelAgent (if provided)
            TravelAgent? agent = null;
            if (newBookiingDTO.TravelAgentName != null)
            {
                agent = await _appDbCotext.TravelAgents
                    .FirstOrDefaultAsync(x => x.Name == newBookiingDTO.TravelAgentName);
            }

            // 4. Customer and Employee Lookup
           

            if (existingBooking == null)
            {
                await UpdateCustomer(newBookiingDTO);
                if (newBookiingDTO.ExternalEmployee != null)
                {
                    await UpdateExternalEmployee(newBookiingDTO);
                }
                var customer = await _appDbCotext.Customers
               .FirstOrDefaultAsync(x => x.CustomerNumber == newBookiingDTO.CustomerNumber);

                var externalEmp = newBookiingDTO.ExternalEmployeeNumber.HasValue
                    ? await _appDbCotext.ExternalEmployees
                        .FirstOrDefaultAsync(x => x.externalEmployeeNumber == newBookiingDTO.ExternalEmployeeNumber.Value)
                    : null;
                // 5. Create new booking
                var newBooking = new Booking
                {
                    From = newBookiingDTO.From,
                    To = newBookiingDTO.To,
                    VehicleId = newBookiingDTO.VehicleId,
                    CustomerID = customer.CustomersId,
                    Userid = newBookiingDTO.UserId,
                    Traveltime = newBookiingDTO.BookingTime,
                    BookingType = bookingType,
                    travelDate = newBookiingDTO.BookingDate,
                    Status = bookingStatus,
                    Pax = newBookiingDTO.Pax,
                    ExternalEmployeeId = externalEmp?.externalEmployeeID,
                    Amount = (decimal)newBookiingDTO.Amount,
                    Payment = paymentSource,
                    TravelAgentId = agent?.AgentId
                };

                await _appDbCotext.Bookings.AddAsync(newBooking);
                await _appDbCotext.SaveChangesAsync();

                // 6. Add Payment Allocation
                var allocations = new List<BookingPaymentAllocation>();

                if (newBookiingDTO.CustomerWillPay > 0)
                {
                    allocations.Add(new BookingPaymentAllocation
                    {
                        BookingId = newBooking.BookingId,
                        PayerType = PayerType.Customer,
                        CustomerId = customer.CustomersId,
                        AllocatedAmount = newBookiingDTO.CustomerWillPay.Value
                    });
                }

                if (newBookiingDTO.OwnerWillPay > 0 && agent != null)
                {
                    var payerType = agent.type == TravelAgentType.TravelOwner ? PayerType.Owner : PayerType.Agent;

                    allocations.Add(new BookingPaymentAllocation
                    {
                        BookingId = newBooking.BookingId,
                        PayerType = payerType,
                        TravelAgentId = agent.AgentId,
                        AllocatedAmount = newBookiingDTO.OwnerWillPay.Value
                    });
                }

                if (allocations.Any())
                {
                    await _appDbCotext.BookingPaymentAllocations.AddRangeAsync(allocations);
                    await _appDbCotext.SaveChangesAsync();
                }

                return newBooking;
            }
            else
            {
                await UpdateCustomer(newBookiingDTO);
                if (newBookiingDTO.ExternalEmployee != null)
                {
                    await UpdateExternalEmployee(newBookiingDTO);
                }

                var externalEmp = newBookiingDTO.ExternalEmployeeNumber.HasValue
                    ? await _appDbCotext.ExternalEmployees
                        .FirstOrDefaultAsync(x => x.externalEmployeeNumber == newBookiingDTO.ExternalEmployeeNumber.Value)
                    : null;
                // 7. Update existing booking
                existingBooking.From = newBookiingDTO.From;
                existingBooking.To = newBookiingDTO.To;
                existingBooking.VehicleId = newBookiingDTO.VehicleId;
                existingBooking.Userid = newBookiingDTO.UserId;
                existingBooking.BookingType = bookingType;
                existingBooking.travelDate = newBookiingDTO.BookingDate;
                existingBooking.Traveltime = newBookiingDTO.BookingTime;
                existingBooking.Status = bookingStatus;
                existingBooking.Pax = newBookiingDTO.Pax;
                existingBooking.Amount = (decimal)newBookiingDTO.Amount;
                existingBooking.Payment = paymentSource;
                existingBooking.ExternalEmployeeId = externalEmp?.externalEmployeeID;
                existingBooking.TravelAgentId = agent?.AgentId;

                _appDbCotext.Bookings.Update(existingBooking);
                await _appDbCotext.SaveChangesAsync();

                // Optional: Update allocations if needed (your choice based on business logic)
                return existingBooking;
            }
        }
        public async Task UpdateExternalEmployee(NewBookiingDTO newBookiingDTO)
        {
            // Validation: number must be valid (non-null and non-zero)
            if (newBookiingDTO.ExternalEmployeeNumber == null || newBookiingDTO.ExternalEmployeeNumber == 0)
                return;

            // Try to find existing employee by number
            var existingEmployee = await _appDbCotext.ExternalEmployees
                .FirstOrDefaultAsync(x => x.externalEmployeeNumber == newBookiingDTO.ExternalEmployeeNumber);

            if (existingEmployee != null)
            {
                // Employee already exists – update name only if it's different
                if (!string.IsNullOrWhiteSpace(newBookiingDTO.ExternalEmployee) &&
                    existingEmployee.externalEmployeeName != newBookiingDTO.ExternalEmployee)
                {
                    existingEmployee.externalEmployeeName = newBookiingDTO.ExternalEmployee;
                    _appDbCotext.ExternalEmployees.Update(existingEmployee);
                    await _appDbCotext.SaveChangesAsync();
                }
            }
            else
            {
                // New employee creation
                var newEmployee = new ExternalEmployee
                {
                    externalEmployeeName = newBookiingDTO.ExternalEmployee,
                    externalEmployeeNumber = newBookiingDTO.ExternalEmployeeNumber.Value
                };

                await _appDbCotext.ExternalEmployees.AddAsync(newEmployee);
                await _appDbCotext.SaveChangesAsync();
            }

        }

        public async Task UpdateCustomer(NewBookiingDTO newBookiingDTO)
        {
            var customer = await _appDbCotext.Customers.FirstOrDefaultAsync(x => x.CustomerNumber == newBookiingDTO.CustomerNumber);
            if (customer == null)
            {
                var newcustomer = new Customers
                {
                    CustomerName = newBookiingDTO.CustomerName,
                    CustomerNumber = newBookiingDTO.CustomerNumber,
                    AlternateNumber = (int)newBookiingDTO.AlternateNumber,
                    TravelDate = newBookiingDTO.BookingDate,
                };
                await _appDbCotext.Customers.AddAsync(newcustomer);
                await _appDbCotext.SaveChangesAsync();
            }
            else if (customer != null)
            {
                // Update existing customer details
                customer.CustomerName = newBookiingDTO.CustomerName;
                customer.CustomerNumber = newBookiingDTO.CustomerNumber;
                customer.AlternateNumber = (int)newBookiingDTO.AlternateNumber;
                customer.TravelDate = newBookiingDTO.BookingDate;
                // Save changes to update the customer
                _appDbCotext.Customers.Update(customer);
                await _appDbCotext.SaveChangesAsync();
            }
        }
        public async Task<List<Booking>> FilterBookingsAsync(IQueryable<Booking> query, BookingFilterDTO filterDTO)
        {

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

            if (!string.IsNullOrEmpty(filterDTO.From))
            {
                query = query.Where(b => b.From == filterDTO.From);
            }

            if (!string.IsNullOrEmpty(filterDTO.To))
            {
                query = query.Where(b => b.To == filterDTO.To);
            }

            if (filterDTO.Status.HasValue)
            {
                query = query.Where(b => b.Status == filterDTO.Status.Value);
            }

            if (filterDTO.VehicleId.HasValue)
            {
                query = query.Where(b => b.VehicleId == filterDTO.VehicleId.Value);
            }

            if (filterDTO.UserId.HasValue)
            {
                query = query.Where(b => b.Userid == filterDTO.UserId.Value);
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
    }
}
