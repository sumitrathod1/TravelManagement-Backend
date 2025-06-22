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
