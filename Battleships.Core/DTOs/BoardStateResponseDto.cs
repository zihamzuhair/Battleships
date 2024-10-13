namespace Battleships.Core.DTOs
{
    public class BoardStateResponseDto
    {
        public List<string> UserBoardDisplay { get; set; } = new List<string>();
        public List<string> ComputerBoardDisplay { get; set; } = new List<string>();
    }
}
