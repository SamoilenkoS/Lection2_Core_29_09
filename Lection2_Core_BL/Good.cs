using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lection2_Core_29_09_API
{
    [Index(nameof(Title), IsUnique = true)]
    public class Good
    {
        [Key]
        [JsonIgnore]
        public Guid Id { get; set; }
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