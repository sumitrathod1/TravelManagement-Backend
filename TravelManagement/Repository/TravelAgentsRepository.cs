using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TravelManagement.AppDBContext;
using TravelManagement.Models;
using TravelManagement.Models.DTO;

namespace TravelManagement.Repository
{
    public class TravelAgentsRepository : ITravelAgentsRepository
    {
        private readonly AppDbContext _context;

        public TravelAgentsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TravelAgent>> GetAllAgentsAsync()
        {
            var agents = await _context.TravelAgents.ToListAsync();

            return agents;
        }

        public async Task<List<AgentDashboardDTO>> GetAllAgentsDashboardAsync()
        {
            var agents = await _context.TravelAgents.ToListAsync();

            var allocations = await _context.BookingPaymentAllocations
                .Where(a => a.PayerType == PayerType.Agent || a.PayerType == PayerType.Owner)
                .ToListAsync();


            var result = agents.Select(agent =>
            {
                var agentAllocations = allocations.Where(a => a.TravelAgentId == agent.AgentId);

                int bookingCount = _context.Bookings.Count(b => b.TravelAgentId == agent.AgentId);

                decimal totalAllocated = agentAllocations.Sum(a => a.AllocatedAmount);
                decimal totalPaid = agentAllocations.Sum(a => a.PaidAmount);

                Console.WriteLine($"Agent {agent.Name} | Allocated: {totalAllocated} | Paid: {totalPaid} | Pending: {totalAllocated - totalPaid}");


                return new AgentDashboardDTO
                {
                    AgentId = agent.AgentId,
                    Name = agent.Name,
                    type = agent.type,
                    BookingCount = bookingCount,
                    Earned = totalPaid,
                    Pending = Math.Max(0, totalAllocated - totalPaid),
                };
            }).ToList();

            return result;
        }
        public async Task<TravelAgent> addAgent(addAgentDTO addAgentDTO)
        {
            TravelAgentType agentType = TravelAgentType.Agent;
            if (addAgentDTO.AgentType == "TravelOwner")
            {
                agentType = TravelAgentType.TravelOwner;
            }
            TravelAgent newAgent = new TravelAgent
            {
                Name = addAgentDTO.Name,
                ContactNumber = addAgentDTO.ContactNumber,
                type = agentType,
                Email = addAgentDTO.Email,


            };
            await _context.TravelAgents.AddAsync(newAgent);
            await _context.SaveChangesAsync();
            return newAgent;
        }

        public async Task<decimal> ApplyAgentPayment(AddAgentPaymentDto dto)
        {

            var unpaidBookings = await _context.Bookings
            .Where(b => b.TravelAgentId == dto.AgentId)
            .Select(b => new { b.BookingId, b.travelDate })
            .OrderBy(b => b.travelDate)
            .ToListAsync();


            decimal remaining = dto.TotalPaidAmount;

            decimal applied = 0;

            foreach (var booking in unpaidBookings.OrderBy(b => b.travelDate))
            {
                var allocation = await _context.BookingPaymentAllocations
                    .FirstOrDefaultAsync(a => a.BookingId == booking.BookingId &&
                                          a.TravelAgentId == dto.AgentId &&
                                          a.PayerType == PayerType.Agent);

                if (allocation == null) continue;

                decimal pending = allocation.AllocatedAmount - allocation.PaidAmount;
                if (pending <= 0 || remaining <= 0) continue;

                decimal toApply = Math.Min(remaining, pending);

                var payment = new Payments
                {
                    AmountPaid = toApply,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = "Cash",
                    BookingId = booking.BookingId,
                    TravelAgentId = dto.AgentId
                };

                _context.Payments.Add(payment);
                var allocationRecord = await _context.BookingPaymentAllocations
                .FirstOrDefaultAsync(a => a.BookingId == booking.BookingId &&
                              a.TravelAgentId == dto.AgentId);

                if (allocationRecord != null)
                {
                    allocationRecord.PaidAmount += toApply;
                }
                remaining -= toApply;
                applied += toApply;
            }

            await _context.SaveChangesAsync();

            return applied;
        }

        public async Task<List<Booking>> GetAgentBookingsById(int agentId)
        {
            var bookings = await _context.Bookings
            .Include(b => b.TravelAgent)
            .Include(b => b.Vehicle)
            .Include(b => b.Customer)
            .Where(b => b.TravelAgentId == agentId).OrderByDescending(b => b.travelDate)
            .ToListAsync();
            return bookings;
        }
        public async Task<List<Booking>> GetAgentReportBookingsById(int agentId, DateOnly? fromDate, DateOnly? toDate)
        {
            var query = _context.Bookings
                .Include(b => b.TravelAgent)
                .Include(b => b.Vehicle)
                .Include(b => b.Customer)
                .Where(b => b.TravelAgentId == agentId);

            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(b => b.travelDate >= fromDate.Value && b.travelDate <= toDate.Value);
            }

            return await query.OrderByDescending(b => b.travelDate).ToListAsync();
        }
    }
}
