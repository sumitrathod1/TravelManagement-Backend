using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
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
                .Where(a => a.PayerType == PayerType.Agent)
                .ToListAsync();

            var result = agents.Select(agent =>
            {
                var agentAllocations = allocations.Where(a => a.TravelAgentId == agent.AgentId);

                int bookingCount = _context.Bookings.Count(b => b.TravelAgentId == agent.AgentId);

                decimal totalAllocated = agentAllocations.Sum(a => a.AllocatedAmount);
                decimal totalPaid = agentAllocations.Sum(a => a.PaidAmount);

                return new AgentDashboardDTO
                {
                    AgentId = agent.AgentId,
                    Name = agent.Name,
                    type=agent.type,
                    BookingCount = bookingCount,
                    Earned = totalPaid,
                    Pending = totalAllocated - totalPaid,
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
                type=agentType,
                Email=addAgentDTO.Email,


            };
            await _context.TravelAgents.AddAsync(newAgent);
            await _context.SaveChangesAsync();
            return newAgent;
        }
    }
}
