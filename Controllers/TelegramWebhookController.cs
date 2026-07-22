using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using wealthify.Services;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("telegram")]
public class TelegramWebhookController(TelegramUpdateHandler handler) : ControllerBase
{

    [HttpPost("webhook")]
    public async Task<IActionResult> Post([FromBody] Update update, CancellationToken ct)
    {
        await handler.HandleAsync(update, ct);
        return Ok();
    }
}
