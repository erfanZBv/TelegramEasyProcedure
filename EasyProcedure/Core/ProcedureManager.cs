using System.Diagnostics.CodeAnalysis;
using EasyProcedure.Contracts;
using EasyProcedure.Exceptions;
using EasyProcedure.Helpers;
using EasyProcedure.JsonModels;
using EasyProcedure.RenderModels;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyProcedure.Core;

public class ProcedureManager : IProcedureManager
{
    private readonly TelegramBotClient _bot;
    private readonly List<string> _supportedLanguages;
    private Dictionary<string, Option> _renderedOptions;
    private Dictionary<string, Stage> _renderedStages;

    public ProcedureManager(TelegramBotClient bot, string botConfigJson)
    {
        _bot = bot;
        var jsonModel = new JsonParser().ParseFromJson(botConfigJson);
        new BotConfigValidator(jsonModel).Validate();

        _supportedLanguages = jsonModel.SupportedLanguages;

        RenderFromJson(jsonModel);
    }

    public Task OnUpdate(Update update)
    {
        if (!TryExtractCallbackData(update, out var data))
            return Task.CompletedTask;

        return OnUpdate(update, data);
    }

    public bool TryGetStageMessage(
        string procedureId, string stageId, string languageTitle,
        [MaybeNullWhen(false)] out IStageMessage renderedStage
    )
    {
        var stageKey = Stage.GenerateDictionaryKey(procedureId, stageId);
        var tryResult = _renderedStages.TryGetValue(stageKey, out var stage);
        renderedStage = stage;
        return tryResult;
    }

    public ProcedureManager AddOptionOnClick(
        string procedureId, string stageId, string optionId,
        Func<TelegramBotClient, Update, OptionOnClickDetails, Task<OptionHandlerResult>> onClick
    )
    {
        var key = Option.GenerateDictionaryKey(procedureId, stageId, optionId);
        if (!_renderedOptions.TryGetValue(key, out var option))
            throw new OptionNotExistException(
                $"Option with ID {optionId} in stage {stageId} in procedure {procedureId} not found.");

        option.OnClick = onClick;
        return this;
    }

    #region private methods

    private void RenderFromJson(BotConfigJsonModel jsonModel)
    {
        var renderedProceduresList = Mapper.ToRenderedProcedures(jsonModel);
        var renderedStagesList = renderedProceduresList.SelectMany(x => x.Stages).ToList();
        var renderedOptionsList = renderedStagesList
            .SelectMany(x => x.OptionMarkup.SelectMany(y => y));

        _renderedStages = new Dictionary<string, Stage>(
            renderedStagesList.Select(x => new KeyValuePair<string, Stage>(x.DictionaryKey, x))
        );
        _renderedOptions = new Dictionary<string, Option>(
            renderedOptionsList.Select(x => new KeyValuePair<string, Option>(x.DictionaryKey, x))
        );
    }

    private bool TryExtractCallbackData(
        Update update,
        [MaybeNullWhen(false)] out ButtonCallbackData buttonCallbackData
    )
    {
        buttonCallbackData = null;

        if (update is not { CallbackQuery.Data: { } rawData } || rawData is null)
            return false;

        return TryParseButtonCallbackData(rawData, out buttonCallbackData);
    }

    private Task OnUpdate(Update update, ButtonCallbackData data)
    {
        if (!_renderedOptions.TryGetValue(data.OptionDictionaryKey, out var option))
            return Task.CompletedTask;

        return OnUpdate(update, option, data.Language);
    }

    private async Task OnUpdate(Update update, Option option, string language)
    {
        var handlerResult = await ExecuteOptionHandler(update, option, language);

        await MoveNextStageIfAllowed(update, option, language, handlerResult);
    }

    private async Task MoveNextStageIfAllowed(
        Update update,
        Option option,
        string language,
        OptionHandlerResult? handlerResult
    )
    {
        if (option.NextStageId == null || handlerResult is { AvoidMovingNextStage : true })
            return;

        if (update is not { CallbackQuery.Message: { } message })
            return;

        var nextStageKey = Stage.GenerateDictionaryKey(option.Stage.Procedure.Id, option.NextStageId);
        if (!_renderedStages.TryGetValue(nextStageKey, out var nextStage))
            return;

        await _bot.EditMessageText(
            chatId: message.Chat.Id,
            messageId: message.MessageId,
            text: nextStage.MultilanguageText[language],
            parseMode: nextStage.TextParseMode,
            replyMarkup: nextStage.MultilanguageReplyMarkup?[language]
        );
    }

    private async Task<OptionHandlerResult?> ExecuteOptionHandler(Update update, Option option, string language)
    {
        if (option.OnClick is null)
            return null;

        var optionOnClickDetails =
            new OptionOnClickDetails(language, option.Stage.Procedure.Id, option.Stage.Id, option.Id);
        return await option.OnClick(_bot, update, optionOnClickDetails);
    }

    private bool TryParseButtonCallbackData(
        string callbackData,
        [MaybeNullWhen(false)] out ButtonCallbackData data
    )
    {
        if (!Option.TryParseButtonCallbackData(callbackData, out data))
            return false;

        // Also check for supported language
        if (_supportedLanguages.Contains(data.Language))
            return true;

        data = null;
        return false;
    }

    #endregion
}