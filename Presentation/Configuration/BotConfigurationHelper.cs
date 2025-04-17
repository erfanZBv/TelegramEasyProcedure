using Telegram.Bot;

namespace Presentation.Configuration;

public static class BotConfigurationHelper
{
    public static TelegramBotClient CreateClient(CancellationToken cancellationToken)
    {
        var botToken = AppSettings.Root.Bot.Token;
        if (string.IsNullOrEmpty(botToken))
            throw new NullReferenceException("Telegram bot token environment variable is missing.");

        return new TelegramBotClient(botToken, cancellationToken: cancellationToken);
    }

    public static async Task StopOnKeyStroke(TelegramBotClient bot, ConsoleKey consoleKey, CancellationTokenSource cts)
    {
        var me = await bot.GetMe();
        Console.WriteLine($"@{me.Username} is running... \tPress '{consoleKey}' to terminate");

        while (Console.ReadKey(true).Key != consoleKey)
        {
        }

        await cts.CancelAsync(); // stop the bot

        Console.WriteLine($"@{me.Username} terminated.");
    }
}