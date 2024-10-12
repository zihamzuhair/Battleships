
using Battleships.Core.Models.Base;

namespace Battleships.Core.Models
{
    public class Fleet : EntityBase<int>
    {
        public int BoardId { get; set; }
        public Board Board { get; set; }
        public List<Ship> Ships { get; set; } = new List<Ship>();
    }
}