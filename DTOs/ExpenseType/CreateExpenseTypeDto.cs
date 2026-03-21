using System.ComponentModel.DataAnnotations;

namespace wealthify.DTOs.ExpenseType;

public record CreateExpenseTypeDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string ExpenseTypeName { get; init; }
}