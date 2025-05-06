using EasyProcedure.Core;
using Presentation.Attributes;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Presentation.Controllers;

public class OnMessageController(TelegramBotClient bot, ProcedureManager procedureManager)
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
        const string language = "english";
        if (!procedureManager.TryGetStageMessage("0", "0", language, out var stageMessage))
            return;

        await bot.SendMessage(
            msg.Chat,
            stageMessage.MultilanguageText[language],
            parseMode: stageMessage.TextParseMode,
            replyMarkup: stageMessage.MultilanguageReplyMarkup?[language]
        );
    }

    [OnMessage("/start_Farsi")]
    public async Task HandleStartPersianCommand(Message msg)
    {
        const string language = "farsi";
        if (!procedureManager.TryGetStageMessage("0", "0", language, out var stageMessage))
            return;

        await bot.SendMessage(
            msg.Chat,
            stageMessage.MultilanguageText[language],
            parseMode: stageMessage.TextParseMode,
            replyMarkup: stageMessage.MultilanguageReplyMarkup?[language]
        );
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
}