using System.ComponentModel.DataAnnotations;

namespace wealthify.DTOs.User;

public record class LoginUserDto
{
    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; init; }

    [Required]
    [StringLength(128, MinimumLength = 6)]
    public string Password { get; init; }
}
