using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using wealthify.Database;
using wealthify.Entity;
using wealthify.Options;

namespace wealthify.Services;

public class TelegramUpdateHandler
{
    private static readonly Regex ExpenseLineRegex = new(
        "^\\s*(?<amount>\\d+(?:\\.\\d{1,2})?)\\s+(?<summary>.+)$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly ApplicationDbContext _db;
    private readonly ITelegramBotClient _bot;
    private readonly ILogger<TelegramUpdateHandler> _logger;
    private readonly ExpenseOptions _expenseOptions;
    private readonly TimeZoneInfo _timeZone;

    public TelegramUpdateHandler(
        ApplicationDbContext db,
        ITelegramBotClient bot,
        IOptions<ExpenseOptions> expenseOptions,
        ILogger<TelegramUpdateHandler> logger)
    {
        _db = db;
        _bot = bot;
        _logger = logger;
        _expenseOptions = expenseOptions.Value;
        _timeZone = ResolveTimeZone(_expenseOptions.TimeZoneId);
    }

    public async Task HandleAsync(Update update, CancellationToken ct)
    {
        if (update.Message?.Text is null)
        {
            return;
        }

        var message = update.Message;
        if (message.Chat.Type != ChatType.Private)
        {
            await SendTextAsync(message.Chat.Id, "Please use this bot in a private chat only.", ct);
            return;
        }

        if (message.From is null)
        {
            return;
        }

        var text = message.Text.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        if (text.StartsWith('/'))
        {
            await HandleCommandAsync(message, text, ct);
            return;
        }

        await HandleExpenseLinesAsync(message, text, ct);
    }

    private async Task HandleCommandAsync(Message message, string text, CancellationToken ct)
    {
        var parts = text.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var commandToken = parts[0];
        var command = commandToken.Split('@')[0].TrimStart('/').ToLowerInvariant();
        var monthInput = parts.Length > 1 ? parts[1].Trim() : null;

        if (command is "total" or "month")
        {
            if (!TryGetMonthRange(monthInput, out var startUtc, out var endUtc, out var label))
            {
                await SendTextAsync(message.Chat.Id, "Invalid month format. Use YYYY-MM (e.g., 2026-06).", ct);
                return;
            }

            var userId = message.From!.Id;
            var chatId = message.Chat.Id;

            if (command == "total")
            {
                var total = await _db.Expenses
                    .Where(e => e.TelegramUserId == userId
                        && e.TelegramChatId == chatId
                        && e.CreatedAtUtc >= startUtc
                        && e.CreatedAtUtc < endUtc)
                    .SumAsync(e => (decimal?)e.Amount, ct) ?? 0m;

                _logger.LogInformation(
                    "Computed total for user {UserId} chat {ChatId} month {MonthLabel}",
                    userId,
                    chatId,
                    label);

                var totalText = total.ToString("0.##", CultureInfo.InvariantCulture);
                await SendTextAsync(message.Chat.Id, $"Total for {label}: {totalText} {_expenseOptions.Currency}", ct);
                return;
            }

            var items = await _db.Expenses
                .Where(e => e.TelegramUserId == userId
                    && e.TelegramChatId == chatId
                    && e.CreatedAtUtc >= startUtc
                    && e.CreatedAtUtc < endUtc)
                .OrderByDescending(e => e.CreatedAtUtc)
                .ToListAsync(ct);

            _logger.LogInformation(
                "Listed {Count} expense(s) for user {UserId} chat {ChatId} month {MonthLabel}",
                items.Count,
                userId,
                chatId,
                label);

            if (items.Count == 0)
            {
                await SendTextAsync(message.Chat.Id, $"No expenses for {label}.", ct);
                return;
            }

            var response = new StringBuilder();
            response.AppendLine($"Expenses for {label} ({_expenseOptions.Currency})");
            foreach (var item in items)
            {
                var localTime = TimeZoneInfo.ConvertTimeFromUtc(item.CreatedAtUtc, _timeZone);
                var amountText = item.Amount.ToString("0.##", CultureInfo.InvariantCulture);
                response.AppendLine($"{localTime:yyyy-MM-dd HH:mm} | {amountText} {_expenseOptions.Currency} | {item.Summary}");
            }

            await SendTextAsync(message.Chat.Id, response.ToString().TrimEnd(), ct);
            return;
        }

        await SendTextAsync(message.Chat.Id, "Unknown command. Use /total or /month.", ct);
    }

    private async Task HandleExpenseLinesAsync(Message message, string text, CancellationToken ct)
    {
        var lines = text.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
        var expenses = new List<Expense>();
        var errors = new List<string>();

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var match = ExpenseLineRegex.Match(line);
            if (!match.Success)
            {
                errors.Add($"\"{line}\" (use: amount summary)");
                continue;
            }

            var amountText = match.Groups["amount"].Value;
            if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount) || amount <= 0)
            {
                errors.Add($"\"{line}\" (invalid amount)");
                continue;
            }

            var summary = match.Groups["summary"].Value.Trim();
            if (summary.Length == 0)
            {
                errors.Add($"\"{line}\" (missing summary)");
                continue;
            }

            expenses.Add(new Expense
            {
                Id = Guid.NewGuid(),
                TelegramUserId = message.From!.Id,
                TelegramChatId = message.Chat.Id,
                Amount = amount,
                Summary = summary,
                CreatedAtUtc = message.Date.ToUniversalTime()
            });
        }

        if (expenses.Count > 0)
        {
            _db.Expenses.AddRange(expenses);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Saved {Count} expense(s) for user {UserId} chat {ChatId}",
                expenses.Count,
                message.From!.Id,
                message.Chat.Id);
        }

        if (expenses.Count == 0 && errors.Count == 0)
        {
            await SendTextAsync(message.Chat.Id, "No expenses found. Use: 120 lunch", ct);
            return;
        }

        var response = new StringBuilder();
        if (expenses.Count > 0)
        {
            response.AppendLine($"Saved {expenses.Count} expense(s).");
        }

        if (errors.Count > 0)
        {
            response.AppendLine("Could not parse:");
            foreach (var error in errors)
            {
                response.AppendLine($"- {error}");
            }
        }

        await SendTextAsync(message.Chat.Id, response.ToString().TrimEnd(), ct);
    }

    private async Task SendTextAsync(long chatId, string text, CancellationToken ct)
    {
        await _bot.SendMessage(chatId, text, cancellationToken: ct);
    }

    private bool TryGetMonthRange(string? monthInput, out DateTime startUtc, out DateTime endUtc, out string label)
    {
        DateTime localStart;

        if (string.IsNullOrWhiteSpace(monthInput))
        {
            var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZone);
            localStart = new DateTime(nowLocal.Year, nowLocal.Month, 1);
        }
        else if (DateTime.TryParseExact(monthInput, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            localStart = new DateTime(parsed.Year, parsed.Month, 1);
        }
        else
        {
            startUtc = default;
            endUtc = default;
            label = string.Empty;
            return false;
        }

        var localEnd = localStart.AddMonths(1);
        startUtc = TimeZoneInfo.ConvertTimeToUtc(localStart, _timeZone);
        endUtc = TimeZoneInfo.ConvertTimeToUtc(localEnd, _timeZone);
        label = localStart.ToString("yyyy-MM", CultureInfo.InvariantCulture);
        return true;
    }

    private static TimeZoneInfo ResolveTimeZone(string timeZoneId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            if (!string.Equals(timeZoneId, "Asia/Kolkata", StringComparison.OrdinalIgnoreCase))
            {
                throw;
            }
        }
        catch (InvalidTimeZoneException)
        {
            if (!string.Equals(timeZoneId, "Asia/Kolkata", StringComparison.OrdinalIgnoreCase))
            {
                throw;
            }
        }

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        }
        catch
        {
            return TimeZoneInfo.Utc;
        }
    }
}
