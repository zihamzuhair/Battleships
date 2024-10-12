using Battleships.Core.Models;

namespace Battleships.DAL.IRepositories
{
    public interface IFleetRepository
    {
        Task SaveFleetAsync(Fleet fleet);
        Task<Fleet> GetFleetAsync(int fleetId);
        Task UpdateFleetAsync(Fleet fleet);
    }
}
