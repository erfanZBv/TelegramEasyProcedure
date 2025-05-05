using EasyProcedure.JsonModels;
using EasyProcedure.RenderModels;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EasyProcedure.Helpers;

internal static class Mapper
{
    internal static IEnumerable<Procedure> ToRenderedProcedures(BotConfigJsonModel validatedJsonModel)
    {
        var renderedProcedures = new List<Procedure>();
        var languages = validatedJsonModel.SupportedLanguages;
        var jsonButtons = new Dictionary<int, ButtonJsonModel>(
            validatedJsonModel.Buttons.Select(x => new KeyValuePair<int, ButtonJsonModel>(x.Id!.Value, x))
        );

        foreach (var jsonProcedure in validatedJsonModel.Procedures)
        {
            var procedure = new Procedure(jsonProcedure.Id!.Value.ToString());
            renderedProcedures.Add(procedure);

            foreach (var jsonStage in jsonProcedure.Stages)
            {
                var stage = new Stage(jsonStage.Id!.Value.ToString(), procedure, jsonStage.Text,
                    ParseMode.None); // only ParseMode.None for now
                procedure.Stages.Add(stage);

                PopulateStageOptions(stage, jsonStage);

                if (!IsRootStage(jsonStage, jsonProcedure))
                {
                    var defaultOptionsRow = new List<Option>();

                    if (jsonProcedure.AddToPreviousButton && !jsonStage.RemoveToPreviousButton)
                        defaultOptionsRow.Add(
                            GenerateToPreviousOption(stage, jsonStage, jsonProcedure.ToPreviousButtonId!.Value)
                        );

                    if (jsonProcedure.AddToRootButton && !jsonStage.RemoveToRootButton)
                        defaultOptionsRow.Add(
                            GenerateToRootOption(stage, jsonProcedure.ToRootButtonId!.Value,
                                jsonProcedure.RootStageId!.Value)
                        );

                    if (defaultOptionsRow.Count != 0)
                        stage.OptionMarkup.Add(defaultOptionsRow);
                }

                stage.MultilanguageReplyMarkup = ToMultilanguageReplyMarkup(stage.OptionMarkup);
            }
        }

        return renderedProcedures;

        void PopulateStageOptions(Stage stage, StageJsonModel jsonStage)
        {
            if (jsonStage.Options.Count == 0)
                return;

            var options = new List<List<Option>>();
            stage.OptionMarkup = options;

            foreach (var jsonOptionRow in jsonStage.Options)
            {
                var optionRow = new List<Option>();
                options.Add(optionRow);

                foreach (var jsonOption in jsonOptionRow)
                {
                    var jsonButton = jsonButtons[jsonOption.ButtonId!.Value];
                    var option = new Option(jsonOption.Id!.Value.ToString(), stage, jsonButton.Text,
                        jsonOption.NextStageId?.ToString());
                    optionRow.Add(option);
                }
            }
        }

        bool IsRootStage(StageJsonModel jsonStage, ProcedureJsonModel jsonProcedure) =>
            jsonStage.Id!.Value == jsonProcedure.RootStageId!.Value;

        Option GenerateToPreviousOption(Stage stage, StageJsonModel jsonStage, int toPreviousButtonId)
        {
            var jsonButton = jsonButtons[toPreviousButtonId];
            return new Option(Constants.ToPreviousOptionId, stage, jsonButton.Text,
                jsonStage.PreviousStageId!.Value.ToString());
        }

        Option GenerateToRootOption(Stage stage, int toRootButtonId, int rootStageId)
        {
            var jsonButton = jsonButtons[toRootButtonId];
            return new Option(Constants.ToRootOptionId, stage, jsonButton.Text, rootStageId.ToString());
        }
    }

    internal static Dictionary<string, InlineKeyboardMarkup> ToMultilanguageReplyMarkup(List<List<Option>> optionMarkup)
    {
        var languageKeyboards = new Dictionary<string, InlineKeyboardMarkup>();

        var allLanguages = optionMarkup
            .SelectMany(row => row)
            .SelectMany(option => option.MultilanguageText.Keys)
            .Distinct();

        foreach (var language in allLanguages)
        {
            var keyboardRows = new List<List<InlineKeyboardButton>>();

            foreach (var optionRow in optionMarkup)
            {
                var rowButtons = new List<InlineKeyboardButton>();

                foreach (var option in optionRow)
                {
                    if (option.MultilanguageText.TryGetValue(language, out var text))
                    {
                        rowButtons.Add(
                            InlineKeyboardButton
                                .WithCallbackData(text, Option.BuildButtonCallbackData(option.DictionaryKey, language))
                        );
                    }
                }

                if (rowButtons.Count > 0)
                {
                    keyboardRows.Add(rowButtons);
                }
            }

            languageKeyboards[language] = new InlineKeyboardMarkup(keyboardRows);
        }

        return languageKeyboards;
    }
}