using wealthify.Entity;

public record InvestmentTypeDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}