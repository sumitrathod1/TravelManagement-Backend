﻿using Microsoft.AspNetCore.Mvc;
using TravelManagement.AppDBContext;
using TravelManagement.Models;
using TravelManagement.Models.DTO;
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
        public async Task <IActionResult> AddAgent([FromBody] addAgentDTO addAgentDTO)
        {
            if (addAgentDTO == null)
            {
                throw new ArgumentNullException(nameof(addAgentDTO), "Travel agent cannot be null.");
            }
            var agent = await _travelAgentsRepository.addAgent(addAgentDTO);
            
            return Ok(new { Message = "Travel Agent successfully added", Agent = agent });
        }

        [HttpGet("GetAllAgent")]
        public async Task<IActionResult> GetAllAgent()
        {
            var agents = await _travelAgentsRepository.GetAllAgentsDashboardAsync();

            if (agents == null || agents.Count == 0)
            {
                return NotFound("No travel agents found.");
            }
            return Ok(agents);
        }

        [HttpPost("ApplyAgentPayment")]
        public async Task<IActionResult> applyAgentAmount([FromBody] AddAgentPaymentDto dto)
        {
            if (dto == null || dto.TotalPaidAmount <=0)
                return BadRequest("Invalid request data.");

            decimal applied= await _travelAgentsRepository.ApplyAgentPayment(dto);

            if (applied <= 0)
                return BadRequest(new { message = "No pending amount to apply for this agent." });

            return Ok(new
            {
                message = $"Payment applied successfully (₹{applied})."
            });
        }
    }
}