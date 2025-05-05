using System.Text.Json.Serialization;

namespace EasyProcedure.JsonModels;

internal class ButtonJsonModel : IWithJsonId, IWithJsonText
{
    [JsonPropertyName("id")] public int? Id { get; set; }

    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("text")] public Dictionary<string, string> Text { get; set; } = new();
}