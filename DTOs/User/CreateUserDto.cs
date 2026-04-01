using System.ComponentModel.DataAnnotations;

namespace wealthify.DTOs.User;

public record class CreateUserDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 6)]
    public string Password { get; set; }
    public bool IsFamilyHead { get; set; } = false;
}
