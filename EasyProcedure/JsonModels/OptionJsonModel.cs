using System.Text.Json.Serialization;

namespace EasyProcedure.JsonModels;

internal class OptionJsonModel : IWithJsonId
{
    [JsonPropertyName("id")] public int? Id { get; set; }

    [JsonPropertyName("buttonId")] public int? ButtonId { get; set; }

    [JsonPropertyName("nextStageId")] public int? NextStageId { get; set; }
}