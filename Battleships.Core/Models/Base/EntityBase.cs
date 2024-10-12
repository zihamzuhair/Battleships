using System.ComponentModel.DataAnnotations;

namespace Battleships.Core.Models.Base
{
    public class EntityBase<T>
    {
        [Key]
        public T Id { get; set; } 
    }
}
