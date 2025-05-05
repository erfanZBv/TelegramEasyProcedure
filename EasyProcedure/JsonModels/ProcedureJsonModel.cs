using System.Text.Json.Serialization;

namespace EasyProcedure.JsonModels;

internal class ProcedureJsonModel : IWithJsonId
{
    [JsonPropertyName("id")] public int? Id { get; set; }

    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("rootStageId")] public int? RootStageId { get; set; }

    [JsonPropertyName("addToRootButton")] public bool AddToRootButton { get; set; }

    [JsonPropertyName("addToPreviousButton")]
    public bool AddToPreviousButton { get; set; }

    [JsonPropertyName("toRootButtonId")] public int? ToRootButtonId { get; set; }

    [JsonPropertyName("toPreviousButtonId")]
    public int? ToPreviousButtonId { get; set; }

    [JsonPropertyName("stages")] public List<StageJsonModel> Stages { get; set; } = [];
}