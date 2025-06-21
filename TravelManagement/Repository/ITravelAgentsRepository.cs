using TravelManagement.Models;

namespace TravelManagement.Repository
{
    public interface ITravelAgentsRepository
    {
        Task <List<TravelAgent>> GetAllAgentsAsync();
        Task <TravelAgent> addAgent(TravelAgent travelAgent);
    }
}
