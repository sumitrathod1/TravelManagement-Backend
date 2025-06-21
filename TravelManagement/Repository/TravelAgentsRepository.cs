using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TravelManagement.AppDBContext;
using TravelManagement.Models;

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
        public async Task<TravelAgent> addAgent(TravelAgent travelAgent)
        {
            await _context.TravelAgents.AddAsync(travelAgent);
            await _context.SaveChangesAsync();
            return travelAgent;
        }
    }
}
