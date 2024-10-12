using Battleships.Core.Models;

namespace Battleships.DAL.IRepositories
{
    public interface IBoardRepository
    {
        Task<Board> GetBoardAsync();
        Task SaveBoardAsync(Board board);
    }
}
