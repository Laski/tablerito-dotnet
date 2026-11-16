using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Tablerito.Services;

namespace Tablerito.Tests;

public class BoardServiceTests
{
    private readonly BoardService boards;

    public BoardServiceTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Tablerito:Notifications:Url"] = "http://localhost:9000/notifications",
                ["Tablerito:Notifications:ApiKey"] = "api-key",
                ["Tablerito:Environment"] = "development",
            })
            .Build();
        var notifications = new NotificationService(new NotificationClient(config, NullLogger<NotificationClient>.Instance), config, NullLogger<NotificationService>.Instance);
        boards = new BoardService(notifications, NullLogger<BoardService>.Instance);
        boards.SeedSampleBoard();
    }

    [Fact]
    public void MovesACardToAnotherList()
    {
        var source = boards.CreateList("To do");
        var target = boards.CreateList("Done");
        var card = boards.CreateCard(source.Id, "Pack the books");

        boards.MoveCard(card.Id, target.Id);

        Assert.Empty(source.Cards);
        Assert.Same(card, Assert.Single(target.Cards));
    }

    [Fact]
    public void AMissingCardIsNotFound()
    {
        var error = Assert.Throws<NotFoundException>(() => boards.FindCard(999));
        Assert.Equal("Card 999 not found", error.Message);
    }
}
