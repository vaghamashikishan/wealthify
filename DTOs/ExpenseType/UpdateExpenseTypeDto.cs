using System.ComponentModel.DataAnnotations;

namespace wealthify.DTOs.ExpenseType;

public record UpdateExpenseTypeDto
{
    [Required]
    public int Id { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string ExpenseTypeName { get; init; }
}