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
        public async Task<Board?> GetBoardByUserIdAsync(int userId)
        {
            return await _context.Boards.FirstOrDefaultAsync(b => b.UserId == userId);
        }

        public async Task<List<Board>> GetAllBoardsByUserIdAsync(int userId)
        {
            return await _context.Boards.Where(b => b.UserId == userId).ToListAsync();
        }

        public async Task<Board> AddBoardAsync(Board board)
        {
            var createdBoard = await _context.Boards.AddAsync(board);
            await _context.SaveChangesAsync();

            return createdBoard.Entity;
        }

        public async Task UpdateBoardAsync(Board board)
        {
            _context.Boards.Update(board);
            await _context.SaveChangesAsync();
        }        
    }
}