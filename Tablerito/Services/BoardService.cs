using Tablerito.Models;

namespace Tablerito.Services;

public class BoardService
{
    private readonly Board board = new("Curso Cloud-Native");
    private long lastId;
    private readonly NotificationService notifications;
    private readonly ILogger<BoardService> logger;

    public BoardService(NotificationService notifications, ILogger<BoardService> logger)
    {
        this.notifications = notifications;
        this.logger = logger;
    }

    public void SeedSampleBoard()
    {
        var toDo = CreateList("Por hacer");
        var doing = CreateList("Haciendo");
        var done = CreateList("Hecho");
        CreateCard(done.Id, "Leer los 12 factores");
        CreateCard(doing.Id, "Encontrar los problemas del Tablerito");
        CreateCard(toDo.Id, "Meter el Tablerito en un contenedor");
        CreateCard(toDo.Id, "Armarle un pipeline");
        logger.LogInformation("Sample board loaded");
    }

    public Board Get()
    {
        return board;
    }

    public CardList CreateList(string name)
    {
        var list = new CardList(NewId(), name);
        board.Lists.Add(list);
        return list;
    }

    public Card CreateCard(long listId, string title)
    {
        var card = new Card(NewId(), title);
        FindList(listId).Cards.Add(card);
        return card;
    }

    public void MoveCard(long cardId, long targetListId)
    {
        var target = FindList(targetListId);
        var card = FindCard(cardId);
        foreach (var list in board.Lists)
        {
            list.Cards.Remove(card);
        }
        target.Cards.Add(card);
        notifications.Notify(new Notification($"La tarjeta \"{card.Title}\" pasó a {target.Name}"));
    }

    public Card FindCard(long cardId)
    {
        foreach (var list in board.Lists)
        {
            foreach (var card in list.Cards)
            {
                if (card.Id == cardId)
                {
                    return card;
                }
            }
        }
        throw new NotFoundException($"Card {cardId} not found");
    }

    private CardList FindList(long listId)
    {
        foreach (var list in board.Lists)
        {
            if (list.Id == listId)
            {
                return list;
            }
        }
        throw new NotFoundException($"List {listId} not found");
    }

    private long NewId()
    {
        return Interlocked.Increment(ref lastId);
    }
}
