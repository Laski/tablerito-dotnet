namespace Tablerito.Models;

public record Board(string Name)
{
    public List<CardList> Lists { get; } = new();
}
