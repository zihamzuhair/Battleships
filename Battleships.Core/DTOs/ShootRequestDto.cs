namespace Battleships.Core.DTOs
{
    public class ShootRequestDto
    {
        public char Row { get; set; }  // Row as a character, e.g., 'A'
        public int Column { get; set; } // Column as an integer, e.g., 5
    }
}
