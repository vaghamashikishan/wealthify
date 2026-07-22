namespace wealthify.Options;

public sealed class TelegramBotOptions
{
    public const string SectionName = "TelegramBot";

    public string Token { get; set; } = string.Empty;

    public string WebhookBaseUrl { get; set; } = string.Empty;
}
