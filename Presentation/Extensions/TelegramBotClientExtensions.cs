using System.Reflection;
using Presentation.Attributes;
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
        var onMessageMethods = GetOnMessageMethods();

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

    private static List<OnMessageMethod> GetOnMessageMethods()
    {
        var methods = typeof(OnMessageController).GetMethods()
            .Select(m => new OnMessageMethod
            {
                Method = m,
                Attribute = m.GetCustomAttribute<OnMessageAttribute>()
            })
            .Where(m => m.Attribute != null)
            .ToList();
        return methods;
    }

    private class OnMessageMethod
    {
        public required MethodInfo Method { get; init; }
        public OnMessageAttribute? Attribute { get; init; }
    }
}