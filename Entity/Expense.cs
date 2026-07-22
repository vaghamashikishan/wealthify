namespace wealthify.Entity;

public class Expense
{
    public Guid Id { get; set; }

    public long TelegramUserId { get; set; }

    public long TelegramChatId { get; set; }

    public decimal Amount { get; set; }

    public string Summary { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}
