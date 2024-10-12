using Battleships.Core.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Battleships.Core.Models
{  
    public class Board : EntityBase<int>
    {
        public string? SerializedGrid { get; set; } // Store the serialized grid as a string

        public int FleetId { get; set; }
     
        [ForeignKey("FleetId")]
        public Fleet Fleet { get; set; }
    }
}