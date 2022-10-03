using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Lection2_Core_DAL.Entities;

[Index(nameof(Title), IsUnique = true)]
public class Role : Entity
{
    [Required]
    public string? Title { get; set; }
}
