using System.ComponentModel.DataAnnotations;

namespace Battleships.Core.Models.Base
{
    public class EntityBase
    {
        [Key]
        public int Id { get; set; }
    }
}
