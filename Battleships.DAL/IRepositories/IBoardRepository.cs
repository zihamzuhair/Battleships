using Battleships.Core.Models;

namespace Battleships.DAL.IRepositories
{
    public interface IBoardRepository
    {
        Task<Board?> GetBoardByUserIdAsync(int userId);
        Task<List<Board>> GetAllBoardsByUserIdAsync(int userId);
        Task<Board> AddBoardAsync(Board board);
        Task UpdateBoardAsync(Board board);
    }
}
