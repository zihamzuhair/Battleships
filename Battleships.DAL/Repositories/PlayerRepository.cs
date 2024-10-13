using Battleships.Core.Models;
using Battleships.DAL.Context;
using Battleships.DAL.IRepositories;
using Microsoft.EntityFrameworkCore;


namespace Battleships.DAL.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly BattleshipDbContext _context;

        public PlayerRepository(BattleshipDbContext context)
        {
            _context = context;
        }
                
        public async Task<List<Player>> GetPlayersByBoardsUserIdAsync(int userId)
        {
            return await _context.Players
                .Include(p => p.Board)
                .Include(f => f.Fleet)
                .ThenInclude(s => s.Ships)
                .Where(p => p.Board.UserId == userId)
                .ToListAsync();
        }

        public async Task AddPlayerAsync(Player player)
        {
            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAllEntitiesyUserIdAsync(int userId)
        {
            var players = await _context.Players
                                            .Include(p => p.Board)
                                            .Include(p => p.Fleet)
                                                .ThenInclude(f => f.Ships)
                                                .Where(P => P.Board.UserId == userId)
                                            .ToListAsync();

            if (players.Any())
            {
                // Remove all entities associated with each player
                foreach (var player in players)
                {
                    if (player.Board != null)
                    {
                        _context.Boards.Remove(player.Board);
                    }

                    if (player.Fleet != null)
                    {
                        _context.Fleets.Remove(player.Fleet); // this removes ships as well
                    }
                }

                _context.Players.RemoveRange(players);
                return await _context.SaveChangesAsync() > 0; 
            }
            return false;
        }
    }
}
