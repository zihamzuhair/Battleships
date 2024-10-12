using Battleships.Core.DTOs;
using Battleships.Core.Models;
using Battleships.DAL.UnitOfWork;
using Battleships.Services.IService;

namespace Battleships.Services
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Random _random;
        private const int BoardSize = 10;

        public GameService(IUnitOfWork unitOfWork, Random random)
        {
            _unitOfWork = unitOfWork;
            _random = random;
        }
        public async Task InitializeGameAsync()
        {
            var board = await _unitOfWork.Boards.GetBoardAsync();

            // Create an empty grid for the board.
            var grid = CreateEmptyGrid();
            board.SerializedGrid = SerializeGrid(grid);

            // Create a new Fleet for the board.
            var fleet = new Fleet();

            // Add ships to the fleet.
            fleet.Ships.Add(new Ship { Name = "Battleship", Size = 5 });
            fleet.Ships.Add(new Ship { Name = "Destroyer1", Size = 4 });
            fleet.Ships.Add(new Ship { Name = "Destroyer2", Size = 4 });

            // Save the fleet and associate with the board.
            await _unitOfWork.Fleets.SaveFleetAsync(fleet);
            board.FleetId = fleet.Id;

     
            // Associate the Fleet with the Board.
            board.FleetId = fleet.Id;
            await _unitOfWork.Boards.SaveBoardAsync(board);

            // Place ships on the board.
            foreach (var ship in fleet.Ships)
            {
                PlaceShip(board, ship);
            }

            // Update the board grid after placing ships.
            board.SerializedGrid = SerializeGrid(grid);
            await _unitOfWork.Boards.SaveBoardAsync(board);

            // Save changes using Unit of Work
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
            var grid = DeserializeGrid(board.SerializedGrid);
            var (row, colIndex) = ParsePosition(rowChar, column);

            bool hit = false;
            if (grid[row, colIndex] == "S")
            {
                grid[row, colIndex] = "X"; // Mark hit
                RegisterHitOnShip(board, row, colIndex);
                hit = true;
            }
            else if (grid[row, colIndex] == "~")
            {
                grid[row, colIndex] = "O"; // Mark miss
            }
            else
            {
                throw new InvalidOperationException("Position already shot.");
            }

            board.SerializedGrid = SerializeGrid(grid);
            await _unitOfWork.Boards.SaveBoardAsync(board);
            await _unitOfWork.CompleteAsync();

            return new ShootResponseDto
            {
                Row = rowChar,
                Column = column,
                Hit = hit,
                GameOver = board.Fleet.Ships.All(ship => ship.Hits >= ship.Size),
                Board = GetBoardDisplay(grid)
            };
        }

        public async Task<string> GetBoardStateAsync()
        {
            var board = await _unitOfWork.Boards.GetBoardAsync();
            var grid = DeserializeGrid(board.SerializedGrid);
            return GetBoardDisplay(grid);
        }

        public async Task ResetGameAsync()
        {
            var board = await _unitOfWork.Boards.GetBoardAsync();
            var emptyGrid = CreateEmptyGrid(); // Create the empty 2D array.
            board.SerializedGrid = SerializeGrid(emptyGrid); // Serialize the 2D array into a string.
            board.Fleet?.Ships.ForEach(s => s.Hits = 0);
            await _unitOfWork.Boards.SaveBoardAsync(board);       
            await _unitOfWork.CompleteAsync();
        }

        private void PlaceShip(Board board, Ship ship)
        {
            var grid = DeserializeGrid(board.SerializedGrid);
            bool placed = false;

            while (!placed)
            {
                int startRow = _random.Next(0, BoardSize);
                int startCol = _random.Next(0, BoardSize);
                bool horizontal = _random.Next(0, 2) == 0;

                if (CanPlaceShip(ship.Size, startRow, startCol, horizontal, grid))
                {
                    for (int i = 0; i < ship.Size; i++)
                    {
                        if (horizontal)
                            grid[startRow, startCol + i] = "S"; // Place ship horizontally.
                        else
                            grid[startRow + i, startCol] = "S"; // Place ship vertically.
                    }

                    placed = true;
                }
            }

            board.SerializedGrid = SerializeGrid(grid);
        }

        private bool CanPlaceShip(int size, int row, int col, bool horizontal, string[,] grid)
        {
            if (horizontal)
            {
                if (col + size > BoardSize) return false;
                for (int i = 0; i < size; i++)
                {
                    if (grid[row, col + i] == "S") return false;
                }
            }
            else
            {
                if (row + size > BoardSize) return false;
                for (int i = 0; i < size; i++)
                {
                    if (grid[row + i, col] == "S") return false;
                }
            }
            return true;
        }


        private void RegisterHitOnShip(Board board, int row, int col)
        {
            foreach (var ship in board.Fleet.Ships)
            {
                ship.Hits++;
            }
        }

        private (int row, int col) ParsePosition(char rowChar, int col)
        {
            int row = char.ToUpper(rowChar) - 'A';
            int column = col - 1;

            if (row < 0 || row >= BoardSize || column < 0 || column >= BoardSize)
                throw new ArgumentException("Invalid position.");

            return (row, column);
        }
        private string[,] CreateEmptyGrid()
        {
            var grid = new string[BoardSize, BoardSize];
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    grid[row, col] = "~";
                }
            }
            return grid;
        }

        private string SerializeGrid(string[,] grid)
        {
            return string.Join(",", grid.Cast<string>());
        }

        private string[,] DeserializeGrid(string serializedGrid)
        {
            var gridValues = serializedGrid.Split(",");
            var grid = new string[10, 10];
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    grid[row, col] = gridValues[row * 10 + col];
                }
            }
            return grid;
        }

        private string GetBoardDisplay(string[,] grid)
        {
            var display = "";
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    display += $"{grid[row, col]} ";
                }
                display += Environment.NewLine;
            }
            return display;
        }
    }
}

