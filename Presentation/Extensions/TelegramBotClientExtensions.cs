using System.Reflection;
using EasyProcedure.Core;
using Presentation.Attributes;
using Presentation.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Presentation.Extensions;

public static class TelegramBotClientExtensions
{
    public static void AddEvents(this TelegramBotClient bot)
    {
        var procedureManager = bot.CreateProcedureManager();

        bot.AddOnMessage(procedureManager);
        bot.AddOnUpdate(procedureManager);
    }

    public static void AddOnMessage(this TelegramBotClient bot, ProcedureManager procedureManager)
    {
        var onMessageController = new OnMessageController(bot, procedureManager);
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

    public static void AddOnUpdate(this TelegramBotClient bot, ProcedureManager procedureManager)
    {
        AddOptionOnClicks(procedureManager);
        bot.OnUpdate += procedureManager.OnUpdate;
    }

    # region private

    private static ProcedureManager CreateProcedureManager(this TelegramBotClient bot)
    {
        var json = File.ReadAllText("./botconfig.json");
        return new ProcedureManager(bot, json);
    }

    private static void AddOptionOnClicks(ProcedureManager procedureManager)
    {
        var controllerType = typeof(OnOptionClickController);
        var methods =
            controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        var controllerInstance = new OnOptionClickController();

        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<OnOptionClickAttribute>();
            if (attr == null) continue;

            var del = (Func<TelegramBotClient, Update, OptionOnClickDetails, Task<OptionHandlerResult>>)
                Delegate.CreateDelegate(
                    typeof(Func<TelegramBotClient, Update, OptionOnClickDetails, Task<OptionHandlerResult>>),
                    controllerInstance,
                    method
                );

            procedureManager.SetOptionOnClick(attr.ProcedureId, attr.StageId, attr.OptionId, del);
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

    #endregion
}