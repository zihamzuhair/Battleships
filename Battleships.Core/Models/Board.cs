using Battleships.Core.Models.Base;

namespace Battleships.Core.Models
{  
    public class Board : EntityBase
    {
        public string? SerializedGrid { get; set; } // Store the serialized grid as a string
        
        // Navigation property for related Ships.
        public List<Ship> Ships { get; set; } = new List<Ship>();
    }
}