using System.Diagnostics.CodeAnalysis;
using EasyProcedure.Core;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EasyProcedure.Contracts;

public interface IProcedureManager
{
    Task OnUpdate(Update update);

    bool TryGetStageMessage(
        string procedureId, string stageId, string languageTitle,
        [MaybeNullWhen(false)] out IStageMessage renderedStage
    );

    ProcedureManager SetOptionOnClick(
        string procedureId, string stageId, string optionId,
        Func<TelegramBotClient, Update, OptionOnClickDetails, Task<OptionHandlerResult>> onClick
    );
}