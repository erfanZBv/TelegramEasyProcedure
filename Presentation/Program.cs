using Presentation;
using Telegram.Bot;

var botToken = AppSettings.Root.Bot.Token;
if (string.IsNullOrEmpty(botToken))
    throw new NullReferenceException("Telegram bot token environment variable is missing.");

using var cts = new CancellationTokenSource();
var botClient = new TelegramBotClient(botToken, cancellationToken: cts.Token);

var bot = await botClient.GetMe();
await botClient.DeleteWebhook();
await botClient.DropPendingUpdates();

