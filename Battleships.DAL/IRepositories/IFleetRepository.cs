using Battleships.Core.Models;

namespace Battleships.DAL.IRepositories
{
    public interface IFleetRepository
    {
        Task SaveFleetAsync(Fleet fleet);
        Task UpdateFleetAsync(Fleet fleet);
    }
}
