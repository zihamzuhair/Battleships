using Battleships.Services.Constants;

namespace Battleships.Services.Helpers
{
    public static class GridHelper
    {
        public static  (int row, int col)  ParsePosition(char rowChar, int col,int boardSize)
        {
            int row = char.ToUpper(rowChar) - 'A';
            int column = col - 1;

            if (row < 0 || row >= boardSize || column < 0 || column >= boardSize)
                throw new ArgumentException("Invalid position.");

            return (row, column);
        }
        public static string[,] CreateEmptyGrid(int boardSize)
        {
            var grid = new string[boardSize, boardSize];
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    grid[row, col] = GlobalConstants.Water;
                }
            }
            return grid;
        }

        public static string SerializeGrid(string[,] grid)
        {
            return string.Join(",", grid.Cast<string>());
        }

        public static string[,] DeserializeGrid(string serializedGrid)
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
    }
}
