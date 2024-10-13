namespace Battleships.Core.DTOs
{
    public class ShootResponseDto
    {
        public char Row { get; set; }
        public int Column { get; set; }
        public bool ComputerHit { get; set; }
        public bool UserHit { get; set; }
        public bool GameOver { get; set; }      
        public int UserScore { get; set; }
        public int ComputerScore { get; set; }
    }
}
