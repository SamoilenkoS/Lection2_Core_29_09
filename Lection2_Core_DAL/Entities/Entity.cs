using System.ComponentModel.DataAnnotations;

namespace Lection2_Core_DAL.Entities
{
    public abstract class Entity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
