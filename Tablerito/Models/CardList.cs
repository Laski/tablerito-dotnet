namespace Tablerito.Models;

public record CardList(long Id, string Name)
{
    public List<Card> Cards { get; } = new();
}
