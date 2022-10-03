using System.ComponentModel.DataAnnotations;

namespace Lection2_Core_BL.DTOs;

public class CredentialsDto
{
    [Required]
    public string? Login { get; set; }
    [Required]
    public string? Password { get; set; }
}
