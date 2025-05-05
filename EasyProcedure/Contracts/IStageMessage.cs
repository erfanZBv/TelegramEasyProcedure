using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EasyProcedure.Contracts;

public interface IStageMessage : IWithMultilanguageText
{
    ParseMode TextParseMode { get; set; }
    Dictionary<string, InlineKeyboardMarkup>? MultilanguageReplyMarkup { get; set; }
}