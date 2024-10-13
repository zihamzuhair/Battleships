
using Battleships.Core.Models.Base;

namespace Battleships.Core.Models
{
    public class Fleet : EntityBase<int>
    {
        public List<Ship> Ships { get; set; } = new List<Ship>(); 
    }
}