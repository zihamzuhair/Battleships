using Battleships.DAL.IRepositories;

namespace Battleships.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IBoardRepository Boards { get; }
        IFleetRepository Fleets { get; }
        IPlayerRepository Players { get; }
        Task<int> CompleteAsync(); 
    }
}
