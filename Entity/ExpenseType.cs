using System;

namespace wealthify.Entity;

public class ExpenseType
{
    public int Id { get; set; }
    public string ExpenseTypeName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
