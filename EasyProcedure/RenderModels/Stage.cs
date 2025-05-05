using EasyProcedure.Contracts;
using EasyProcedure.Core;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EasyProcedure.RenderModels;

internal class Stage : IStageMessage
{
    public Stage(
        string id,
        Procedure procedure,
        Dictionary<string, string> multilanguageText,
        ParseMode textParseMode,
        Dictionary<string, InlineKeyboardMarkup>? multilanguageReplyMarkup = null
    )
    {
        Id = id;
        Procedure = procedure;
        MultilanguageText = multilanguageText;
        TextParseMode = textParseMode;
        MultilanguageReplyMarkup = multilanguageReplyMarkup;
        DictionaryKey = GenerateDictionaryKey(Procedure.Id, Id);
    }

    public static string GenerateDictionaryKey(string procedureId, string stageId)
        => DictionaryUtils.GenerateStringKey(procedureId, stageId);

    public string DictionaryKey { get; }
    public string Id { get; init; }
    public Procedure Procedure { get; init; }
    public Dictionary<string, string> MultilanguageText { get; set; }
    public ParseMode TextParseMode { get; set; }
    public Dictionary<string, InlineKeyboardMarkup>? MultilanguageReplyMarkup { get; set; }
    public List<List<Option>> OptionMarkup { get; set; } = [];
}