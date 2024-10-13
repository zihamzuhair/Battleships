using Battleships.Core.Models.Base;

namespace Battleships.Core.Models
{
    public class Player : EntityBase<int>
    {
        public string Name { get; set; } = string.Empty;
        public bool IsComputer { get; set; }
        public int Score { get; set; } = 0;
        public int BoardId { get; set; }
        public Board Board { get; set; } = null!;
        public int FleetId { get; set; } 
        public Fleet Fleet { get; set; } = null!;
    }
}
