using Presentation.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Presentation.Extensions;

public static class TelegramBotClientExtensions
{
    public static void AddOnMessage(this TelegramBotClient bot)
    {
        var onMessageController = new OnMessageController(bot);
        var onMessageMethods = OnMessageController.GetOnMessageMethods();

        bot.OnMessage += OnMessage;
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
    }
}