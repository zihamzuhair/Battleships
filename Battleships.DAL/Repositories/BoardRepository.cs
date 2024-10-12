using Battleships.Core.Models;
using Battleships.DAL.Context;
using Battleships.DAL.IRepositories;
using Microsoft.EntityFrameworkCore;


namespace Battleships.DAL.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly BattleshipDbContext _context;

        public BoardRepository(BattleshipDbContext context)
        {
            _context = context;
        }

        public async Task<Board> GetBoardAsync()
        {
            var board = await _context.Boards
                .Include(b => b.Fleet)
                .ThenInclude(f => f.Ships)
                .FirstOrDefaultAsync();
            if (board == null)
            {
                board = new Board();
                await _context.Boards.AddAsync(board);
                await _context.SaveChangesAsync();
            }
            return board;
        }

        public async Task SaveBoardAsync(Board board)
        {
            _context.Boards.Update(board);
            await _context.SaveChangesAsync();
        }
    }
}