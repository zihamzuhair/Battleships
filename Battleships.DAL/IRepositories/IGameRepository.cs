using Battleships.Core.Models;

namespace Battleships.DAL.IRepositories
{
    public interface IGameRepository
    {
        Task<Board> GetBoardAsync();
        Task SaveBoardAsync(Board board);
    }
}
