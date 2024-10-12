using Battleships.Core.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Battleships.Core.Models
{
    public class Ship : EntityBase<int>
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public int Hits { get; set; }

        [ForeignKey("Board")]
        public int BoardId { get; set; }

        [ForeignKey("Fleet")]
        public int FleetId { get; set; }
        public Fleet Fleet { get; set; }
    }
}
