using Battleships.Core.Models;
using Battleships.Services.Constants;

namespace Battleships.Services.Helpers
{
    public static class ShipHelper
    {
        private static readonly Random _random = new Random();

        // Method to place multiple ships
        public static void PlaceShips(Board board, List<Ship> ships, int boardSize)
        {
            foreach (var ship in ships)
            {
                PlaceShip(board, ship, boardSize);  
            }
        }

        // Method to register hits on ships
        public static void RegisterHitOnShip(Board board, int row, int col)
        {
            foreach (var ship in board.Fleet.Ships)
            {
                ship.Hits++; // Increment hit count (may need refinement depending on hit detection logic)
            }
        }

        // Method to place a single ship on the board
        private static void PlaceShip(Board board, Ship ship, int boardSize)
        {
            var grid = GridHelper.DeserializeGrid(board.SerializedGrid!);
            bool placed = false;

            // Try placing the ship until it's successfully placed
            while (!placed)
            {
                int startRow = _random.Next(0, boardSize);
                int startCol = _random.Next(0, boardSize);
                bool horizontal = _random.Next(0, 2) == 0;

                // Check if the ship can be placed at the current position
                if (CanPlaceShip(ship.Size, startRow, startCol, horizontal, grid, boardSize))
                {
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

            board.SerializedGrid = GridHelper.SerializeGrid(grid);
        }

        // Helper method to check if a ship can be placed at a given position
        private static bool CanPlaceShip(int shipSize, int row, int col, bool horizontal, string[,] grid, int boardSize)
        {
            if (horizontal)
            {
                if (col + shipSize > boardSize) return false; // Out of bounds
                for (int i = 0; i < shipSize; i++)
                {
                    if (grid[row, col + i] == GlobalConstants.Ship) return false; // Space is already occupied
                }
            }
            else
            {
                if (row + shipSize > boardSize) return false; // Out of bounds
                for (int i = 0; i < shipSize; i++)
                {
                    if (grid[row + i, col] == GlobalConstants.Ship) return false; // Space is already occupied
                }
            }
            return true;
        }    
    }
}
