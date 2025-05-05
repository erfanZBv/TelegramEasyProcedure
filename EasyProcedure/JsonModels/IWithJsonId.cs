using System.Text.Json.Serialization;

namespace EasyProcedure.JsonModels;

internal interface IWithJsonId
{
    [JsonPropertyName("id")] public int? Id { get; set; }
}