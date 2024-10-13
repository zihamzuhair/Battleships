using Battleships.Core.Models;

namespace Battleships.DAL.IRepositories
{
    public interface IPlayerRepository
    {
        Task<List<Player>> GetPlayersByBoardsUserIdAsync(int userId);
        Task AddPlayerAsync(Player player);
        Task<bool> RemoveAllEntitiesyUserIdAsync(int userId);
    }
}
