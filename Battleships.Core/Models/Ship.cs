using Battleships.Core.Models.Base;

namespace Battleships.Core.Models
{
    public class Ship : EntityBase<int>
    {
        public string Name { get; set; } = string.Empty;
        public int Size { get; set; }
        public int Hits { get; set; }
        public int FleetId { get; set; }
        public Fleet Fleet { get; set; } = null!;
    }
}
