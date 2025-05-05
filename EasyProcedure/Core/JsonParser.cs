using System.Text.Json;
using System.Text.Json.Serialization;
using EasyProcedure.Exceptions;
using EasyProcedure.JsonModels;

namespace EasyProcedure.Core;

internal class JsonParser
{
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonParser()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };
    }

    public BotConfigJsonModel ParseFromJson(string json)
    {
        var result = JsonSerializer.Deserialize<BotConfigJsonModel>(json, _jsonOptions);
        if (result is null)
            throw new NullBotConfigException();
        return result;
    }
}