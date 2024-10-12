namespace Battleships.Core.DTOs
{
    public class ShootResponseDto
    {
        public char Row { get; set; }
        public int Column { get; set; }
        public bool Hit { get; set; }
        public bool GameOver { get; set; }
        public string Board { get; set; } // New property to include the board state
    }
}
