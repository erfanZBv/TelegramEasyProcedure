using EasyProcedure.Core;
using Presentation.Attributes;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Presentation.Controllers;

public class OnOptionClickController
{
    [OnOptionClick(0, 0, 0)]
    public Task<OptionHandlerResult> OnClickExample(
        TelegramBotClient bot, Update update, OptionOnClickDetails details
    )
    {
        Console.WriteLine("Clicked!!");
        return Task.FromResult(new OptionHandlerResult(true));
    }
}