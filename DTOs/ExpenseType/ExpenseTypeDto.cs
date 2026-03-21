namespace wealthify.DTOs.ExpenseType;

public record ExpenseTypeDto
{
    public int Id { get; init; }
    public string ExpenseTypeName { get; init; } = string.Empty;
}