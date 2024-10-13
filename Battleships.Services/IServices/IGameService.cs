using Battleships.Core.DTOs;

namespace Battleships.Services.IServices
{
    public interface IGameService
    {
        Task InitializeGameAsync(int userId);
        Task<bool> IsGameInitiatedAsync(int userId);
        Task<ShootResponseDto> ShootAsync(int userId, char row, int column);
        Task<BoardStateResponseDto> GetBoardStateWithShipsAsync(int userId,bool showPlacedShips);
        Task ResetGameAsync(int userId);
        Task QuitGameAsync(int userId);
    }
}
