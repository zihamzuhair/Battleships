using Battleships.Core.DTOs;
using Battleships.Core.Models;
using Battleships.DAL.UnitOfWork;
using Battleships.Services.Helpers;
using Battleships.Services.IService;
using System.Text;
using Battleships.Services.Constants;

namespace Battleships.Services
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int BoardSize = GlobalConstants.DefaultBoardSize;

        public GameService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task InitializeGameAsync()
        {
            var board = await _unitOfWork.Boards.GetBoardAsync();

            // Create an empty grid for the board.
            var grid = GridHelper.CreateEmptyGrid(BoardSize);
            board.SerializedGrid = GridHelper.SerializeGrid(grid);

            var fleet = new Fleet();

            // Add ships to the fleet.
            fleet.Ships.Add(new Ship { Name = "Battleship", Size = 5 });
            fleet.Ships.Add(new Ship { Name = "Destroyer1", Size = 4 });
            fleet.Ships.Add(new Ship { Name = "Destroyer2", Size = 4 });

            // Save the fleet and associate with the board.
            await _unitOfWork.Fleets.SaveFleetAsync(fleet);
            board.FleetId = fleet.Id;

       
            board.FleetId = fleet.Id;
            await _unitOfWork.Boards.SaveBoardAsync(board);

            ShipHelper.PlaceShips(board, fleet.Ships, BoardSize);

            // Update the board after placing ships.
            await _unitOfWork.Boards.SaveBoardAsync(board);

        
            await _unitOfWork.CompleteAsync();
        }

        // New method to check if the game is initiated
        public async Task<bool> IsGameInitiatedAsync()
        {
            var board = await _unitOfWork.Boards.GetBoardAsync();
            return board != null && board.FleetId != 0; // Check if the board and its fleet exist
        }

        public async Task<ShootResponseDto> ShootAsync(char rowChar, int column)
        {
            var board = await _unitOfWork.Boards.GetBoardAsync();
            var grid = GridHelper.DeserializeGrid(board.SerializedGrid!);
            var (row, colIndex) = GridHelper.ParsePosition(rowChar, column, BoardSize);

            bool hit = false;
            if (grid[row, colIndex] == GlobalConstants.Ship)
            {
                grid[row, colIndex] = GlobalConstants.Hit; // Mark hit
                ShipHelper.RegisterHitOnShip(board, row, colIndex);
                hit = true;
            }
            else if (grid[row, colIndex] == GlobalConstants.Water)
            {
                grid[row, colIndex] = GlobalConstants.Miss; // Mark miss
            }
            else
            {
                throw new InvalidOperationException("Position already shot.");
            }

            board.SerializedGrid = GridHelper.SerializeGrid(grid);
            await _unitOfWork.Boards.SaveBoardAsync(board);
            await _unitOfWork.CompleteAsync();

            return new ShootResponseDto
            {
                Row = rowChar,
                Column = column,
                Hit = hit,
                GameOver = board.Fleet.Ships.All(ship => ship.Hits >= ship.Size)
            };
        }

        public async Task<string> GetBoardStateWithShipsAsync(bool showPlacedShips)
        {
            var board = await _unitOfWork.Boards.GetBoardAsync();
            var grid = GridHelper.DeserializeGrid(board.SerializedGrid!);
            return GetBoardDisplay(grid, showPlacedShips);
        }

        public async Task ResetGameAsync()
        {
            var board = await _unitOfWork.Boards.GetBoardAsync();
            var emptyGrid = GridHelper.CreateEmptyGrid(BoardSize); // Create the empty 2D array.
            board.SerializedGrid = GridHelper.SerializeGrid(emptyGrid); // Serialize the 2D array into a string.
            board.Fleet?.Ships.ForEach(s => s.Hits = 0);
            await _unitOfWork.Boards.SaveBoardAsync(board);
            await _unitOfWork.CompleteAsync();
        }

        private string GetBoardDisplay(string[,] grid, bool showPlacedShips)
        {
            var display = new StringBuilder();

            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    // If we are not showing placed ships and the grid value is "S", display "~"
                    if (!showPlacedShips && grid[row, col] == GlobalConstants.Ship)
                    {
                        display.Append($"{GlobalConstants.Water} ");
                    }
                    else
                    {
                        display.Append($"{grid[row, col]} ");
                    }
                }
                display.AppendLine(); 
            }

            return display.ToString();
        }
    }
}
