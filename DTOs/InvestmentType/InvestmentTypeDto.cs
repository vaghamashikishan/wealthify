namespace wealthify.DTOs.InvestmentType;

public record InvestmentTypeDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}