using System.Text.RegularExpressions;
using EasyProcedure.Core;
using EasyProcedure.Exceptions;
using EasyProcedure.JsonModels;

namespace EasyProcedure.Helpers;

internal class BotConfigValidator(BotConfigJsonModel config)
{
    private readonly HashSet<string> _procedureIds = [];
    private readonly HashSet<string> _stageIds = [];
    private readonly HashSet<string> _optionIds = [];
    private readonly HashSet<string> _buttonIds = [];

    public void Validate()
    {
        ValidateIds();
        ValidateSupportedLanguages();
        ValidateTexts();
        ValidateForeignKeys();
        ValidateRequiredNavigationButtons();
        ValidateNullableIds();
    }

    private static string GetProcedureIdsKey(int? procedureId)
    {
        return procedureId == null ? string.Empty : procedureId.Value.ToString();
    }

    private static string GetStageIdsKey(int? procedureId, int? stageId)
    {
        return procedureId == null || stageId == null ? string.Empty : $"{procedureId.Value}_{stageId.Value}";
    }

    private static string GetOptionIdsKey(int? procedureId, int? stageId, int? optionId)
    {
        return procedureId == null || stageId == null || optionId == null
            ? string.Empty
            : $"{procedureId.Value}_{stageId.Value}_{optionId.Value}";
    }

    private static string GetButtonIdsKey(int? buttonId)
    {
        return buttonId == null ? string.Empty : buttonId.Value.ToString();
    }

    private static string GetLastIdOfKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        var parts = key.Split('_');
        return parts.Length == 0 ? string.Empty : parts[^1];
    }

    private void ValidateIds()
    {
        foreach (var procedure in config.Procedures)
        {
            var procedureKey = GetProcedureIdsKey(procedure.Id);
            ValidateUniqueId(procedureKey, _procedureIds, "procedure");

            foreach (var stage in procedure.Stages)
            {
                var stageKey = GetStageIdsKey(procedure.Id, stage.Id);
                ValidateUniqueId(stageKey, _stageIds, "stage", $" in Procedure ID: {procedure.Id}");

                foreach (var optionGroup in stage.Options)
                {
                    foreach (var option in optionGroup)
                    {
                        var optionKey = GetOptionIdsKey(procedure.Id, stage.Id, option.Id);
                        ValidateUniqueId(optionKey, _optionIds, "option",
                            $" in Stage ID: {stage.Id}, Procedure ID: {procedure.Id}");
                    }
                }
            }
        }

        foreach (var button in config.Buttons)
        {
            var buttonKey = GetButtonIdsKey(button.Id);
            ValidateUniqueId(buttonKey, _buttonIds, "button");
        }
    }

    private static void ValidateUniqueId(string key, HashSet<string> existingIds, string entityName,
        string? extraMessage = null)
    {
        if (string.IsNullOrEmpty(key))
            throw new NoIdSpecifiedException($"No ID specified for {entityName}." + extraMessage);

        if (!existingIds.Add(key))
            throw new IdDuplicationException($"Duplicate ID for {entityName} detected: {GetLastIdOfKey(key)}" +
                                             extraMessage);
    }

    private void ValidateSupportedLanguages()
    {
        var languageSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var lang in config.SupportedLanguages)
        {
            if (!Regex.IsMatch(lang, @"^[a-zA-Z0-9]+$"))
                throw new InvalidLanguageTitleException(
                    $"Language '{lang}' contains invalid characters." +
                    " Only English letters and numbers are allowed for language titles.");

            var maxLangLength = Constants.MaximumLanguageTitleLength;
            if (lang.Length > maxLangLength)
                throw new InvalidLanguageTitleException(
                    $"Language title '{lang}' exceeds the maximum length of {maxLangLength} characters.");

            if (!languageSet.Add(lang))
                throw new DuplicateLanguageException($"Duplicate language detected (case-insensitive): '{lang}'");
        }
    }

    private void ValidateTexts()
    {
        foreach (var procedure in config.Procedures)
        {
            foreach (var stage in procedure.Stages)
            {
                ValidateText(stage.Text, "Stage", stage.Id!.Value.ToString(), $" (procedure {procedure.Id!.Value})");
            }
        }

        foreach (var button in config.Buttons)
        {
            ValidateText(button.Text, "Button", button.Id!.Value.ToString());
        }
    }

    private void ValidateText(Dictionary<string, string> text, string entityType, string entityId,
        string? extraMessage = null)
    {
        foreach (var lang in config.SupportedLanguages)
        {
            if (!text.ContainsKey(lang))
            {
                throw new NoTextSpecifiedForLanguageException(
                    $"{entityType} ID {entityId} is missing text for language: {lang}" + extraMessage);
            }
        }
    }

    private void ValidateForeignKeys()
    {
        foreach (var procedure in config.Procedures)
        {
            if (procedure.RootStageId is null ||
                !_stageIds.Contains(GetStageIdsKey(procedure.Id, procedure.RootStageId)))
                throw new StageNotExistException(
                    $"{nameof(procedure.RootStageId)} {procedure.RootStageId} does not exist in procedure {procedure.Id}");

            if (procedure.ToRootButtonId is not null && !_buttonIds.Contains(GetButtonIdsKey(procedure.ToRootButtonId)))
                throw new ButtonNotExistException(
                    $"{nameof(procedure.ToRootButtonId)} {procedure.ToRootButtonId} does not exist (procedure {procedure.Id})");

            if (procedure.ToPreviousButtonId is not null &&
                !_buttonIds.Contains(GetButtonIdsKey(procedure.ToPreviousButtonId)))
                throw new ButtonNotExistException(
                    $"{nameof(procedure.ToPreviousButtonId)} {procedure.ToPreviousButtonId} does not exist (procedure {procedure.Id})");

            foreach (var stage in procedure.Stages)
            {
                if (stage.PreviousStageId is not null &&
                    !_stageIds.Contains(GetStageIdsKey(procedure.Id, stage.PreviousStageId)))
                {
                    throw new StageNotExistException(
                        $"{nameof(stage.PreviousStageId)} {stage.PreviousStageId} does not exist in procedure {procedure.Id}");
                }

                foreach (var optionGroup in stage.Options)
                {
                    foreach (var option in optionGroup)
                    {
                        if (option.ButtonId is not null && !_buttonIds.Contains(GetButtonIdsKey(option.ButtonId)))
                        {
                            throw new ButtonNotExistException(
                                $"{nameof(option.ButtonId)} {option.ButtonId} does not exist.");
                        }

                        if (option.NextStageId is not null &&
                            !_stageIds.Contains(GetStageIdsKey(procedure.Id, option.NextStageId)))
                        {
                            throw new StageNotExistException(
                                $"{nameof(option.NextStageId)} {option.NextStageId} does not exist in procedure {procedure.Id}");
                        }
                    }
                }
            }
        }
    }

    private void ValidateRequiredNavigationButtons()
    {
        foreach (var procedure in config.Procedures)
        {
            switch (procedure)
            {
                case { AddToRootButton: true, ToRootButtonId: null }:
                    throw new NoRootButtonSpecifiedException(
                        $"{nameof(procedure.AddToRootButton)} is true but {nameof(procedure.ToRootButtonId)} is null.");
                case { AddToPreviousButton: true, ToPreviousButtonId: null }:
                    throw new NoPreviousButtonSpecifiedException(
                        $"{nameof(procedure.AddToPreviousButton)} is true but {nameof(procedure.ToPreviousButtonId)} is null.");
            }
        }
    }

    private void ValidateNullableIds()
    {
        foreach (var procedure in config.Procedures)
        {
            if (procedure.Id == null)
                throw new NoIdSpecifiedException("Procedure has no ID.");

            foreach (var stage in procedure.Stages)
            {
                if (stage.Id == null)
                    throw new NoIdSpecifiedException("Stage has no ID.");

                if (stage.PreviousStageId == null && procedure.RootStageId != stage.Id)
                    throw new NoPreviousStageSpecifiedException("Stage is not root and has no PreviousStageId.");

                foreach (var optionGroup in stage.Options)
                {
                    foreach (var option in optionGroup)
                    {
                        if (option.Id == null)
                            throw new NoIdSpecifiedException("Option has no ID.");

                        if (option.ButtonId == null)
                            throw new NoButtonSpecifiedException($"Option has no {nameof(option.ButtonId)}.");
                    }
                }
            }
        }

        foreach (var button in config.Buttons)
        {
            if (button.Id == null)
                throw new NoIdSpecifiedException("Button has no ID.");
        }
    }
}