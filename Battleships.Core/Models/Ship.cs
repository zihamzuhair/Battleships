using Battleships.Core.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Battleships.Core.Models
{
    public class Ship : EntityBase
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public int Hits { get; set; }

        // Foreign key for the related Board.
        [ForeignKey("Board")]
        public int BoardId { get; set; }
    }
}
