using System.ComponentModel.DataAnnotations;

namespace wealthify.DTOs.InvestmentType;

public record CreateInvestmentTypeDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; init; }
}
