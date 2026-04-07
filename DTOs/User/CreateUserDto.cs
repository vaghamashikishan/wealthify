using System.ComponentModel.DataAnnotations;

namespace wealthify.DTOs.User;

public record class CreateUserDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; init; }

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; init; }

    [Required]
    [StringLength(128, MinimumLength = 6)]
    public string Password { get; init; }
    public bool IsFamilyHead { get; init; } = false;
}
