using Battleships.Core.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Battleships.Core.Models
{  
    public class Board : EntityBase<int>
    {
        public string? SerializedGrid { get; set; } // Store the serialized grid as a string

        // Foreign key for the Fleet.
        public int FleetId { get; set; }

        // Navigation property for the Fleet.
        [ForeignKey("FleetId")]
        public Fleet Fleet { get; set; }
    }
}