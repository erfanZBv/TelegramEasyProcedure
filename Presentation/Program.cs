using Presentation.Configuration;
using Presentation.Extensions;
using Telegram.Bot;

using var cts = new CancellationTokenSource();
var bot = BotConfigurationHelper.CreateClient(cts.Token);

await bot.DeleteWebhook();
await bot.DropPendingUpdates();

bot.AddEvents();

await BotConfigurationHelper.StopOnKeyStroke(bot, ConsoleKey.Q, cts);