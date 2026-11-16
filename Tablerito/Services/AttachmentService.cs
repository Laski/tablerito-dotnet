namespace Tablerito.Services;

public class AttachmentService
{
    public string Save(long cardId, IFormFile file)
    {
        var name = $"{cardId}-{Path.GetFileName(file.FileName)}";
        var target = Path.Combine("attachments", name);
        Directory.CreateDirectory("attachments");
        using var output = File.Create(target);
        file.CopyTo(output);
        return name;
    }

    public string Find(string name)
    {
        var folder = Path.GetFullPath("attachments") + Path.DirectorySeparatorChar;
        var file = Path.GetFullPath(Path.Combine(folder, name));
        if (!file.StartsWith(folder) || !File.Exists(file))
        {
            throw new NotFoundException($"Attachment {name} not found");
        }
        return file;
    }
}
