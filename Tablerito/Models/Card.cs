namespace Tablerito.Models;

public record Card(long Id, string Title)
{
    public List<string> Attachments { get; } = new();
}
