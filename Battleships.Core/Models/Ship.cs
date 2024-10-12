using Battleships.Core.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Battleships.Core.Models
{
    public class Ship : EntityBase<int>
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public int Hits { get; set; }

        // Foreign key for the related Board.
        [ForeignKey("Board")]
        public int BoardId { get; set; }

        // Foreign key for the related Fleet.
        [ForeignKey("Fleet")]
        public int FleetId { get; set; }

        // Navigation property to the Fleet.
        public Fleet Fleet { get; set; }
    }
}
