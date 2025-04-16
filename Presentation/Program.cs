using Presentation;
using Presentation.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botToken = AppSettings.Root.Bot.Token;
if (string.IsNullOrEmpty(botToken))
    throw new NullReferenceException("Telegram bot token environment variable is missing.");

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient(botToken, cancellationToken: cts.Token);

var me = await bot.GetMe();
await bot.DeleteWebhook();
await bot.DropPendingUpdates();

var onMessageController = new OnMessageController(bot);
var onMessageMethods = OnMessageController.GetOnMessageMethods();

bot.OnMessage += OnMessage;

Console.WriteLine($"@{me.Username} is running... \tPress 'q' to terminate");
while (Console.ReadKey(true).Key != ConsoleKey.Q)
{
}

cts.Cancel(); // stop the bot
Console.WriteLine($"@{me.Username} terminated.");
return;

async Task OnMessage(Message msg, UpdateType type)
{
    onMessageController.InitialHandle(msg, type);
    if (msg.Text is null) return;

    var matchedMethod = onMessageMethods
        .FirstOrDefault(x => x.Attribute!.Text.Equals(msg.Text, StringComparison.OrdinalIgnoreCase));
    if (matchedMethod == default)
    {
        await onMessageController.HandleNoMatchingMessage(msg);
        return;
    }

    await (Task)matchedMethod.Method.Invoke(onMessageController, [msg])!;
}