using System.Text.Json.Serialization;

namespace EasyProcedure.JsonModels;

public interface IWithJsonText
{
    [JsonPropertyName("text")] public Dictionary<string, string> Text { get; set; }
}