using System.Text.Json.Serialization;

namespace EasyProcedure.JsonModels;

internal class BotConfigJsonModel
{
    [JsonPropertyName("supportedLanguages")]
    public List<string> SupportedLanguages { get; set; } = [];

    [JsonPropertyName("procedures")] public List<ProcedureJsonModel> Procedures { get; set; } = [];

    [JsonPropertyName("buttons")] public List<ButtonJsonModel> Buttons { get; set; } = [];
}