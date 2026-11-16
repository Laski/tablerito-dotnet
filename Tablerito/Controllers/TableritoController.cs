using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Tablerito.Services;

namespace Tablerito.Controllers;

public class TableritoController : Controller
{
    private readonly BoardService boards;
    private readonly AttachmentService attachments;

    public TableritoController(BoardService boards, AttachmentService attachments)
    {
        this.boards = boards;
        this.attachments = attachments;
    }

    [HttpGet("/")]
    public IActionResult Board()
    {
        return View(boards.Get());
    }

    [HttpPost("/lists/{listId}/cards")]
    public IActionResult CreateCard(long listId, string title)
    {
        boards.CreateCard(listId, title);
        return Redirect("/");
    }

    [HttpPost("/cards/{cardId}/move")]
    public IActionResult MoveCard(long cardId, long listId)
    {
        boards.MoveCard(cardId, listId);
        return Redirect("/");
    }

    [HttpPost("/cards/{cardId}/attachments")]
    public IActionResult Attach(long cardId, IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            var card = boards.FindCard(cardId);
            card.Attachments.Add(attachments.Save(cardId, file));
        }
        return Redirect("/");
    }

    [HttpGet("/attachments/{name}")]
    public IActionResult Download(string name)
    {
        return PhysicalFile(attachments.Find(name), "application/octet-stream", name);
    }

    // If the list, the card or the attachment doesn't exist, answer 404 instead of 500
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is NotFoundException)
        {
            context.Result = NotFound(context.Exception.Message);
            context.ExceptionHandled = true;
        }
    }
}
