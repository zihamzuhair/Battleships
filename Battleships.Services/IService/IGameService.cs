using Battleships.Core.DTOs;

namespace Battleships.Services.IService
{
    public interface IGameService
    {
        Task InitializeGameAsync();
        Task<bool> IsGameInitiatedAsync();
        Task<ShootResponseDto> ShootAsync(char row, int column);
        Task<string> GetBoardStateAsync();
        Task ResetGameAsync();
    }
}
