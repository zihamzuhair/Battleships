using System.ComponentModel.DataAnnotations;

namespace Battleships.Core.Models.Base
{
    public abstract class EntityBase<T>
    {
        [Key]
        public T Id { get; set; } 
    }
}
