using Battleships.DAL.IRepositories;


namespace Battleships.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IBoardRepository Boards { get; }
        IFleetRepository Fleets { get; }
        Task<int> CompleteAsync(); // Save changes to the database
    }
}
