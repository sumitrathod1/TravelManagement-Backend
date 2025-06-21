using Microsoft.AspNetCore.Mvc;
using TravelManagement.AppDBContext;
using TravelManagement.Models;
using TravelManagement.Repository;

namespace TravelManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TravelAgentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ITravelAgentsRepository _travelAgentsRepository;
        public TravelAgentsController(AppDbContext context,ITravelAgentsRepository travelAgentsRepository)
        {
            _context = context;
            _travelAgentsRepository = travelAgentsRepository;
        }

        [HttpPost("AddAgent")]
        public async Task <IActionResult> AddAgent([FromBody] TravelAgent travelAgent)
        {
            if (travelAgent == null)
            {
                throw new ArgumentNullException(nameof(travelAgent), "Travel agent cannot be null.");
            }
            var agent = await _travelAgentsRepository.addAgent(travelAgent);
            
            return Ok(new { Message = "Travel Agent successfully added", Agent = agent });
        }

        [HttpGet("GetAllAgent")]

        public async Task<IActionResult> GetAllAgent()
        {
            var agents = await _travelAgentsRepository.GetAllAgentsAsync();

            if (agents == null || agents.Count == 0)
            {
                return NotFound("No travel agents found.");
            }
            return Ok(agents);
        }
    }
}
