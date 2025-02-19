﻿using Battleships.Core.DTOs;
using Battleships.Core.Models;
using Battleships.DAL.UnitOfWork;
using Battleships.Services.IServices;
using Battleships.Services.Constants;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Battleships.Services
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GameService> _logger;
        private readonly Random _random;
        private const int BoardSize = GlobalConstants.DefaultBoardSize;

        public GameService(IUnitOfWork unitOfWork, ILogger<GameService> logger, Random random)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _random = random;
        }

        public async Task InitializeGameAsync(int userId)
        {
            try
            {
                var existingBoard = await _unitOfWork.Boards.GetBoardByUserIdAsync(userId);
                if (existingBoard != null)
                {
                    throw new InvalidOperationException("A game has already been initialized for this user.");
                }

                var emptyGrid = CreateEmptyGrid();
                var serializedEmptyGrid = SerializeGrid(emptyGrid);

                var boardForPlayer = await _unitOfWork.Boards.AddBoardAsync(new Board() { SerializedGrid = serializedEmptyGrid, UserId = userId });
                var boardForComputer = await _unitOfWork.Boards.AddBoardAsync(new Board() { SerializedGrid = serializedEmptyGrid, UserId = userId });

                var userPlayer = new Player { Name = "User:" + userId, IsComputer = false, BoardId = boardForPlayer.Id };
                var computerPlayer = new Player { Name = "Computer:" + userId, IsComputer = true, BoardId = boardForComputer.Id };

                var userBoard = InitializeBoardForPlayer(boardForPlayer, userPlayer);
                var computerBoard = InitializeBoardForPlayer(boardForComputer, computerPlayer);

                await _unitOfWork.Players.AddPlayerAsync(userPlayer);
                await _unitOfWork.Players.AddPlayerAsync(computerPlayer);

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize game for user {UserId}", userId);
                throw;
            }
        }
               
        public async Task<bool> IsGameInitiatedAsync(int userId)
        {
            try
            {
                var boards = await _unitOfWork.Boards.GetAllBoardsByUserIdAsync(userId);
                return boards.Count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if game is initiated for user {UserId}", userId);
                throw;
            }
        }

        public async Task<ShootResponseDto> ShootAsync(int userId, char rowChar, int column)
        {
            try
            {
                var players = await _unitOfWork.Players.GetPlayersByBoardsUserIdAsync(userId);

                if (players.Count != 2)
                {
                    throw new InvalidOperationException("The user not found.");
                }

                var userPlayer = players.FirstOrDefault(d => d.IsComputer == false);
                var computerPlayer = players.FirstOrDefault(d => d.IsComputer == true);

                var userHit = ProcessShot(computerPlayer!, userPlayer!, rowChar, column);
                var computerShot = GenerateRandomShot();
                var computerHit = ProcessShot(userPlayer!, computerPlayer!, computerShot.Row, computerShot.Column);

                await _unitOfWork.Boards.UpdateBoardAsync(userPlayer!.Board);
                await _unitOfWork.Boards.UpdateBoardAsync(computerPlayer!.Board);
                await _unitOfWork.CompleteAsync();

                return new ShootResponseDto
                {
                    Row = rowChar,
                    Column = column,
                    UserHit = userHit,
                    ComputerHit = computerHit,
                    GameOver = computerPlayer.Fleet.Ships.All(ship => ship.Hits >= ship.Size) || userPlayer.Fleet.Ships.All(ship => ship.Hits >= ship.Size),
                    UserScore = userPlayer.Score,  
                    ComputerScore = computerPlayer.Score 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process shot for user {UserId}", userId);
                throw;
            }
        }

        public async Task<BoardStateResponseDto> GetBoardStateWithShipsAsync(int userId, bool showPlacedShips)
        {
            try
            {
                var players = await _unitOfWork.Players.GetPlayersByBoardsUserIdAsync(userId);

                var userPlayer = players.FirstOrDefault(d => d.IsComputer == false);
                var computerPlayer = players.FirstOrDefault(d => d.IsComputer == true);

                if (userPlayer == null || computerPlayer == null)
                {
                    throw new InvalidOperationException("Board not found for user or computer.");
                }

                var gridList = new List<string[,]>
                {
                    DeserializeGrid(userPlayer.Board.SerializedGrid!),
                    DeserializeGrid(computerPlayer.Board.SerializedGrid!)
                };

                var (userBoardDisplay, computerBoardDisplay) = GetGridDisplays(gridList, showPlacedShips);

                return new BoardStateResponseDto
                {
                    UserBoardDisplay = userBoardDisplay,
                    ComputerBoardDisplay = computerBoardDisplay
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve board state for user {UserId}", userId);
                throw;
            }
        }

        public async Task ResetGameAsync(int userId)
        {
            try
            {
                var players = await _unitOfWork.Players.GetPlayersByBoardsUserIdAsync(userId);

                if (players == null)
                {
                    throw new InvalidOperationException("Board not found to reset.");
                }

                var userPlayer = players.FirstOrDefault(d => d.IsComputer == false);
                var computerPlayer = players.FirstOrDefault(d => d.IsComputer == true);

                if (userPlayer != null)
                {
                    userPlayer.Board.SerializedGrid = ResetGrid(userPlayer.Board.SerializedGrid);
                    userPlayer.Fleet?.Ships.ForEach(s => s.Hits = 0);
                    await _unitOfWork.Boards.UpdateBoardAsync(userPlayer.Board);
                }

                if (computerPlayer != null)
                {
                    computerPlayer.Board.SerializedGrid = ResetGrid(computerPlayer.Board.SerializedGrid);
                    computerPlayer.Fleet?.Ships.ForEach(s => s.Hits = 0);
                    await _unitOfWork.Boards.UpdateBoardAsync(computerPlayer.Board);
                }

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reset game for user {UserId}", userId);
                throw;
            }
        }

        public async Task QuitGameAsync(int userId)
        {
            try
            {
                await _unitOfWork.Players.RemoveAllEntitiesyUserIdAsync(userId);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to quit game for user {UserId}", userId);
                throw;
            }
        }

        #region Grid helpers
        public static string[,] CreateEmptyGrid()
        {
            try
            {
                var grid = new string[BoardSize, BoardSize];
                for (int row = 0; row < BoardSize; row++)
                {
                    for (int col = 0; col < BoardSize; col++)
                    {
                        grid[row, col] = GlobalConstants.Water;
                    }
                }
                return grid;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create empty grid", ex);
            }
        }

        public static string ResetGrid(string gridString)
        {
            try
            {
                return gridString.Replace("X", "S").Replace("O", "~");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to reset grid", ex);
            }
        }

        public static string SerializeGrid(string[,] grid)
        {
            try
            {
                return string.Join(",", grid.Cast<string>());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to serialize grid", ex);
            }
        }

        public static string[,] DeserializeGrid(string serializedGrid)
        {
            try
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
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to deserialize grid", ex);
            }
        }

        public static (List<string> userBoardDisplay, List<string> computerBoardDisplay) GetGridDisplays(List<string[,]> grids, bool showPlacedShips)
        {
            try
            {
                if (grids.Count != 2)
                {
                    throw new ArgumentException("There must be exactly two boards (user and computer) in the list.");
                }

                var userBoardDisplay = new List<string>();
                var computerBoardDisplay = new List<string>();

                for (int row = 0; row < BoardSize; row++)
                {
                    var userRowDisplay = new StringBuilder();
                    var computerRowDisplay = new StringBuilder();

                    for (int col = 0; col < BoardSize; col++)
                    {
                        // User board
                        if (!showPlacedShips && grids[0][row, col] == GlobalConstants.Ship)
                        {
                            userRowDisplay.Append($"{GlobalConstants.Water} ");
                        }
                        else
                        {
                            userRowDisplay.Append($"{grids[0][row, col]} ");
                        }

                        // Computer board
                        if (!showPlacedShips && grids[1][row, col] == GlobalConstants.Ship)
                        {
                            computerRowDisplay.Append($"{GlobalConstants.Water} ");
                        }
                        else
                        {
                            computerRowDisplay.Append($"{grids[1][row, col]} ");
                        }
                    }

                    userBoardDisplay.Add(userRowDisplay.ToString().TrimEnd());
                    computerBoardDisplay.Add(computerRowDisplay.ToString().TrimEnd());
                }

                return (userBoardDisplay, computerBoardDisplay);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to generate grid displays", ex);
            }
        }

        public static (int row, int col) ParsePosition(char rowChar, int col)
        {
            try
            {
                int row = char.ToUpper(rowChar) - 'A';
                int column = col - 1;

                if (row < 0 || row >= BoardSize || column < 0 || column >= BoardSize)
                    throw new ArgumentException("Invalid position.");

                return (row, column);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to parse position", ex);
            }
        }
        #endregion Grid Helpers end

        #region Player helpers

        private Board InitializeBoardForPlayer(Board board, Player player)
        {
            try
            {
                var fleet = new Fleet();

                fleet.Ships.Add(new Ship { Name = "Battleship", Size = 5 });
                fleet.Ships.Add(new Ship { Name = "Destroyer1", Size = 4 });
                fleet.Ships.Add(new Ship { Name = "Destroyer2", Size = 4 });

                player.Fleet = fleet;
                PlaceShips(board, fleet.Ships);

                return board;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize board for opponent {PlayerName}", player.Name);
                throw;
            }
        }

        public static (char Row, int Column) GenerateRandomShot()
        {
            try
            {
                var random = new Random();
                var rowChar = (char)('A' + random.Next(0, BoardSize));
                var column = random.Next(1, BoardSize + 1);
                return (rowChar, column);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to generate random shot", ex);
            }
        }

        public bool ProcessShot(Player opponent, Player shooter, char rowChar, int column)
        {
            try
            {
                var grid = DeserializeGrid(opponent.Board.SerializedGrid!);
                var (row, colIndex) = ParsePosition(rowChar, column);

                bool hit = false;
                if (grid[row, colIndex] == GlobalConstants.Ship)
                {
                    grid[row, colIndex] = GlobalConstants.Hit;
                    RegisterHitOnShip(opponent, row, colIndex);
                    hit = true;

                    // Add 15 points for hitting a ship
                    shooter.Score += 15;

                    // Check if the ship is fully destroyed and add 50 points
                    var hitShip = opponent.Fleet.Ships.FirstOrDefault(ship => ship.Hits >= ship.Size);
                    if (hitShip != null && hitShip.Hits == hitShip.Size)
                    {
                        shooter.Score += 50;
                    }
                }
                else if (grid[row, colIndex] == GlobalConstants.Water)
                {
                    grid[row, colIndex] = GlobalConstants.Miss;
                }
                else
                {
                    throw new InvalidOperationException("Position already shot.");
                }

                opponent.Board.SerializedGrid = SerializeGrid(grid);
                return hit;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process shot");
                throw;
            }
        }

        #endregion Player Helpers end

        #region Ship helpers

        public void PlaceShips(Board board, List<Ship> ships)
        {
            try
            {
                foreach (var ship in ships)
                {
                    PlaceShip(board, ship);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to place ships");
                throw;
            }
        }

        public static void RegisterHitOnShip(Player player, int row, int col)
        {
            try
            {
                // Iterate through all ships and check if the hit (row, col) matches any ship's position
                foreach (var ship in player.Fleet.Ships)
                {
                    // Check if the hit position is within the bounds of the current ship
                    bool isHit = false;

                    if (ship.IsHorizontal)
                    {
                        // Check if the hit is within the horizontal bounds of the ship
                        isHit = row == ship.StartRow && col >= ship.StartCol && col < ship.StartCol + ship.Size;
                    }
                    else
                    {
                        // Check if the hit is within the vertical bounds of the ship
                        isHit = col == ship.StartCol && row >= ship.StartRow && row < ship.StartRow + ship.Size;
                    }

                    // If the hit matches the ship's position, increase the hit count for that ship
                    if (isHit)
                    {
                        ship.Hits++;
                        break; // Exit loop after finding the hit ship
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to register hit on ship", ex);
            }
        }

        private void PlaceShip(Board board, Ship ship)
        {
            try
            {
                var grid = DeserializeGrid(board.SerializedGrid!);
                bool placed = false;

                while (!placed)
                {
                    int startRow = _random.Next(0, BoardSize);
                    int startCol = _random.Next(0, BoardSize);
                    bool horizontal = _random.Next(0, 2) == 0;

                    if (CanPlaceShip(ship.Size, startRow, startCol, horizontal, grid))
                    {
                        // Store the ship's starting position and orientation
                        ship.StartRow = startRow;
                        ship.StartCol = startCol;
                        ship.IsHorizontal = horizontal;

                        for (int i = 0; i < ship.Size; i++)
                        {
                            if (horizontal)
                                grid[startRow, startCol + i] = GlobalConstants.Ship; // Place ship horizontally
                            else
                                grid[startRow + i, startCol] = GlobalConstants.Ship; // Place ship vertically
                        }

                        placed = true;
                    }
                }

                board.SerializedGrid = SerializeGrid(grid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to place ship");
                throw;
            }
        }

        private bool CanPlaceShip(int shipSize, int row, int col, bool horizontal, string[,] grid)
        {
            try
            {
                if (horizontal)
                {
                    if (col + shipSize > BoardSize) return false; // Out of bounds
                    for (int i = 0; i < shipSize; i++)
                    {
                        if (grid[row, col + i] == GlobalConstants.Ship) return false; // Space is already occupied
                    }
                }
                else
                {
                    if (row + shipSize > BoardSize) return false; // Out of bounds
                    for (int i = 0; i < shipSize; i++)
                    {
                        if (grid[row + i, col] == GlobalConstants.Ship) return false; // Space is already occupied
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to check if ship can be placed", ex);
            }
        }
        #endregion Ship Helpers end
    }
}
