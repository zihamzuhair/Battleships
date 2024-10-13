using Battleships.Core.Models.Base;

namespace Battleships.Core.Models
{
    public class Board : EntityBase<int>
    {
        public string SerializedGrid { get; set; } = string.Empty;
        public int UserId { get; set; } 
    }
}