using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Lection2_Core_DAL.Entities;

[Index(nameof(Email), IsUnique = true)]
public class User : Entity
{
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    [Required]
    public DateTime DoB { get; set; }
    [Required]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsDeleted { get; set; }
    public IEnumerable<UserRoles>? Roles { get; set; }
}
