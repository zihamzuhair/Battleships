using Battleships.DAL.Context;
using Battleships.DAL.IRepositories;
using Battleships.DAL.Repositories;

namespace Battleships.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BattleshipDbContext _context;

        public UnitOfWork(BattleshipDbContext context)
        {
            _context = context;
            Boards = new BoardRepository(_context);
            Fleets = new FleetRepository(_context);
            Players = new PlayerRepository(_context);      
        }

        public IBoardRepository Boards { get; private set; }
        public IFleetRepository Fleets { get; private set; }
        public IPlayerRepository Players { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
