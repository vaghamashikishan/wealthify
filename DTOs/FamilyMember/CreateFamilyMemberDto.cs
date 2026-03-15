using System.ComponentModel.DataAnnotations;

namespace wealthify.DTOs.FamilyMember;

public record CreateFamilyMemberDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; init; }
}
