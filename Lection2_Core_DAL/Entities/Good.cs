using Lection2_Core_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Lection2_Core_DAL
{
    [Index(nameof(Title), IsUnique = true)]
    public class Good : Entity
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string? Title { get; set; }
        [Required]
        [Range(0.01, 10_000, ErrorMessage = "Please enter valid price")]
        public double Price { get; set; }
        [Required]
        public Category Category { get; set; }
        public string? Description { get; set; }
    }
}