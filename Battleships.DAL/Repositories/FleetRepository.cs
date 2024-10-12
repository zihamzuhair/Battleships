using Battleships.Core.Models;
using Battleships.DAL.Context;
using Battleships.DAL.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Battleships.DAL.Repositories
{
    public class FleetRepository : IFleetRepository
    {
        private readonly BattleshipDbContext _context;

        public FleetRepository(BattleshipDbContext context)
        {
            _context = context;
        }

        public async Task SaveFleetAsync(Fleet fleet)
        {
            _context.Fleets.Add(fleet);
            await _context.SaveChangesAsync();
        }

        public async Task<Fleet> GetFleetAsync(int fleetId)
        {
            return await _context.Fleets
                .Include(f => f.Ships) // Include the ships in the fleet.
                .FirstOrDefaultAsync(f => f.Id == fleetId);
        }

        public async Task UpdateFleetAsync(Fleet fleet)
        {
            _context.Fleets.Update(fleet);
            await _context.SaveChangesAsync();
        }
    }
}
