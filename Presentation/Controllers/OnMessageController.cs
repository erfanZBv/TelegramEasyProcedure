using System.Reflection;
using Presentation.Attributes;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Presentation.Controllers;

public class OnMessageController(TelegramBotClient bot)
{
    [OnMessage("/start")]
    public async Task HandleStartCommand(Message msg)
    {
        await bot.SendMessage(msg.Chat, """
                                        <b><u>Bot menu</u></b>:
                                        /start_English
                                        /start_Farsi (فارسی)
                                        """, parseMode: ParseMode.Html, linkPreviewOptions: true,
            replyMarkup: new ReplyKeyboardRemove());
    }

    [OnMessage("/start_English")]
    public async Task HandleStartEnglishCommand(Message msg)
    {
        await bot.SendMessage(msg.Chat, """
                                        <b>Hi!</b>
                                        Welcome to Quran bot...
                                        """, parseMode: ParseMode.Html, linkPreviewOptions: true,
            replyMarkup: new ReplyKeyboardRemove());
    }

    [OnMessage("/start_Farsi")]
    public async Task HandleStartPersianCommand(Message msg)
    {
        await bot.SendMessage(msg.Chat, """
                                        <b>سلام!</b>
                                        به بات قرآن خوش آمدید...
                                        """, parseMode: ParseMode.Html, linkPreviewOptions: true,
            replyMarkup: new ReplyKeyboardRemove());
    }

    public void InitialHandle(Message msg, UpdateType type)
    {
        Console.WriteLine("******************");
        Console.WriteLine("New message from " + msg.Chat.Username);
        Console.WriteLine("Message text: " + msg.Text);
        Console.WriteLine("Update type: " + type);
    }

    public async Task HandleNoMatchingMessage(Message msg)
    {
        await bot.SendMessage(msg.Chat, "What?");
    }

    public static List<OnMessageMethod> GetOnMessageMethods()
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
}

public class OnMessageMethod
{
    public required MethodInfo Method { get; set; }
    public OnMessageAttribute? Attribute { get; set; }
}