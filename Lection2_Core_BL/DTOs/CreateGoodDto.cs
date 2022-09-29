using System.ComponentModel.DataAnnotations;

namespace Lection2_Core_BL.DTOs
{
    public class CreateGoodDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string? Title { get; set; }
        [Required]
        [Range(0.01, 10_000, ErrorMessage = "Please enter valid price")]
        public double Price { get; set; }
        [Required]
        public string? Category { get; set; }
        public string? Description { get; set; }
    }
}
