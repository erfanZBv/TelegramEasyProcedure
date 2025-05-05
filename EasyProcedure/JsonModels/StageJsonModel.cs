using System.Text.Json.Serialization;

namespace EasyProcedure.JsonModels;

internal class StageJsonModel : IWithJsonId, IWithJsonText
{
    [JsonPropertyName("id")] public int? Id { get; set; }

    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("previousStageId")] public int? PreviousStageId { get; set; }

    [JsonPropertyName("removeToRootButton")]
    public bool RemoveToRootButton { get; set; }

    [JsonPropertyName("removeToPreviousButton")]
    public bool RemoveToPreviousButton { get; set; }

    [JsonPropertyName("text")] public Dictionary<string, string> Text { get; set; } = new();

    [JsonPropertyName("options")] public List<List<OptionJsonModel>> Options { get; set; } = [];
}