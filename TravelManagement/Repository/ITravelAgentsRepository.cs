using TravelManagement.Models;
using TravelManagement.Models.DTO;

namespace TravelManagement.Repository
{
    public interface ITravelAgentsRepository
    {
        Task <List<TravelAgent>> GetAllAgentsAsync();
        Task <TravelAgent> addAgent(addAgentDTO addAgentDTO);

        Task<List<AgentDashboardDTO>> GetAllAgentsDashboardAsync();
    }
}
