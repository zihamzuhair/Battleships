using Battleships.Core.DTOs;

namespace Battleships.Services.IService
{
    public interface IGameService
    {
        Task InitializeGameAsync();
        Task<ShootResponseDto> ShootAsync(char row, int column);
        Task<string> GetBoardStateAsync();
        Task ResetGameAsync();
    }
}
