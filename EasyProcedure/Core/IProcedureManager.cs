using System.Diagnostics.CodeAnalysis;
using EasyProcedure.Contracts;
using EasyProcedure.RenderModels;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyProcedure.Core;

public interface IProcedureManager
{
    Task OnUpdate(Update update);

    bool TryGetStageMessage(
        string procedureId, string stageId, string languageTitle,
        [MaybeNullWhen(false)] out IStageMessage renderedStage
    );

    ProcedureManager AddOptionOnClick(
        string procedureId, string stageId, string optionId,
        Func<TelegramBotClient, Update, OptionOnClickDetails, Task<OptionHandlerResult>> onClick
    );
}