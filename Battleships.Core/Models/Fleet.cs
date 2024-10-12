
using Battleships.Core.Models.Base;

namespace Battleships.Core.Models
{
    public class Fleet : EntityBase<int>
    {
        // Foreign key for the related Board.
        public int BoardId { get; set; }

        // Navigation property to the Board.
        public Board Board { get; set; }

        // Navigation property for the Ships in the Fleet.
        public List<Ship> Ships { get; set; } = new List<Ship>();
    }
}