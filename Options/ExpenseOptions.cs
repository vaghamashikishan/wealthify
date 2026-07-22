namespace wealthify.Options;

public sealed class ExpenseOptions
{
    public const string SectionName = "ExpenseSettings";

    public string TimeZoneId { get; set; } = "Asia/Kolkata";

    public string Currency { get; set; } = "INR";
}
