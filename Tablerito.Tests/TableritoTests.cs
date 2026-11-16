using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Tablerito.Services;

namespace Tablerito.Tests;

public class TableritoTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient http;
    private readonly BoardService boards;

    public TableritoTests(WebApplicationFactory<Program> app)
    {
        http = app.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        boards = app.Services.GetRequiredService<BoardService>();
    }

    [Fact]
    public async Task StartsWithTheSampleBoard()
    {
        var response = await http.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var page = await response.Content.ReadAsStringAsync();
        Assert.Contains("Curso Cloud-Native", page);
        foreach (var text in new[]
        {
            "Por hacer", "Haciendo", "Hecho",
            "Leer los 12 factores", "Encontrar los problemas del Tablerito",
            "Meter el Tablerito en un contenedor", "Armarle un pipeline",
        })
        {
            Assert.Contains(text, page);
        }
    }

    [Fact]
    public async Task CreatesACardFromThePage()
    {
        var list = boards.CreateList("From the page");

        var form = new FormUrlEncodedContent(new Dictionary<string, string> { ["title"] = "Try out Tablerito" });
        var response = await http.PostAsync($"/lists/{list.Id}/cards", form);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        var page = await http.GetStringAsync("/");
        Assert.Contains("Try out Tablerito", page);
    }

    [Fact]
    public async Task MovesACardFromThePage()
    {
        var source = boards.CreateList("Moving from");
        var target = boards.CreateList("Moving to");
        var card = boards.CreateCard(source.Id, "Pack the books");

        var form = new FormUrlEncodedContent(new Dictionary<string, string> { ["listId"] = target.Id.ToString() });
        var response = await http.PostAsync($"/cards/{card.Id}/move", form);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        Assert.Empty(source.Cards);
        Assert.Same(card, Assert.Single(target.Cards));
    }

    [Fact]
    public async Task SavesAndReturnsAnAttachment()
    {
        var list = boards.CreateList("With attachments");
        var card = boards.CreateCard(list.Id, "Read the contract");
        var form = new MultipartFormDataContent { { new StringContent("hello"), "file", "contract.txt" } };

        var response = await http.PostAsync($"/cards/{card.Id}/attachments", form);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        var name = $"{card.Id}-contract.txt";
        Assert.Equal(name, Assert.Single(card.Attachments));
        Assert.Equal("hello", await http.GetStringAsync($"/attachments/{name}"));

        File.Delete(Path.Combine("attachments", name));
    }

    [Fact]
    public async Task AMissingListReturns404()
    {
        var form = new FormUrlEncodedContent(new Dictionary<string, string> { ["title"] = "Nowhere" });
        var response = await http.PostAsync("/lists/999/cards", form);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
