using System.Diagnostics.CodeAnalysis;
using EasyProcedure.Contracts;
using EasyProcedure.Core;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyProcedure.RenderModels;

internal class Option : IWithMultilanguageText
{
    public Option(
        string id,
        Stage stage,
        Dictionary<string, string> multilanguageText,
        string? nextStageId = null
    )
    {
        Id = id;
        Stage = stage;
        MultilanguageText = multilanguageText;
        NextStageId = nextStageId;
        DictionaryKey = GenerateDictionaryKey(Stage.Procedure.Id, Stage.Id, Id);
    }

    public static string GenerateDictionaryKey(string procedureId, string stageId, string optionId)
        => DictionaryUtils.GenerateStringKey(procedureId, stageId, optionId);

    public string DictionaryKey { get; }
    public string Id { get; }
    public Stage Stage { get; set; }
    public Dictionary<string, string> MultilanguageText { get; set; }
    public string? NextStageId { get; set; }
    public Func<TelegramBotClient, Update, OptionOnClickDetails, Task<OptionHandlerResult>>? OnClick { get; set; }

    public static string BuildButtonCallbackData(string optionDictionaryKey, string language)
        => optionDictionaryKey + "\n" + language;

    public static bool TryParseButtonCallbackData(
        string callbackData,
        [MaybeNullWhen(false)] out ButtonCallbackData data
    )
    {
        data = null;

        if (string.IsNullOrWhiteSpace(callbackData))
            return false;

        var callbackDataLines = callbackData.Split('\n');
        if (callbackDataLines.Length != 2)
            return false;

        data = new ButtonCallbackData(OptionDictionaryKey: callbackDataLines[0], Language: callbackDataLines[1]);
        return true;
    }
}