namespace Presentation.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class OnMessageAttribute(string text) : Attribute
{
    public string Text { get; } = text;
}