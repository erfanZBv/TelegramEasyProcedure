using EasyProcedure.Core;
using EasyProcedure.Helpers;

namespace EasyProcedure.RenderModels;

internal class Procedure
{
    public Procedure(string id)
    {
        Id = id;
        DictionaryKey = GenerateDictionaryKey(Id);
    }

    public static string GenerateDictionaryKey(string procedureId)
        => DictionaryUtils.GenerateStringKey(procedureId);

    public string DictionaryKey { get; }

    public string Id { get; }

    public List<Stage> Stages { get; set; } = [];
}