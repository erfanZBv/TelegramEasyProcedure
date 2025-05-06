namespace Presentation.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class OnOptionClickAttribute : Attribute
{
    public string ProcedureId { get; init; }
    public string StageId { get; init; }
    public string OptionId { get; init; }

    public OnOptionClickAttribute(int procedureId, int stageId, int optionId)
    {
        ProcedureId = procedureId.ToString();
        StageId = stageId.ToString();
        OptionId = optionId.ToString();
    }

    public OnOptionClickAttribute(int procedureId, int stageId, string optionId)
    {
        ProcedureId = procedureId.ToString();
        StageId = stageId.ToString();
        OptionId = optionId;
    }

    public OnOptionClickAttribute(string procedureId, string stageId, string optionId)
    {
        ProcedureId = procedureId;
        StageId = stageId;
        OptionId = optionId;
    }
}