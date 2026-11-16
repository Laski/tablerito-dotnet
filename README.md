# Tablerito

A kanban-style task board: one board with lists and cards, with attachments. It is the sample app of the
Infrastructure and Cloud-Native course, and it has problems on purpose: finding them is the
activity of session 2.

.NET 10 and ASP.NET Core MVC. It opens with Visual Studio (`Tablerito.sln`), Rider or VS Code. The
tools are in `mise.toml`.

## Running it

With [mise](https://mise.jdx.dev), which installs the right .NET SDK, the commands are the same on
Windows, Mac and Linux:

```bash
mise install
mise run dev
```

`mise install` is only needed the first time; it asks whether you trust the repo's `mise.toml`.
Tablerito takes a few seconds to start. Then open http://localhost:8080.

Without mise you need the .NET 10 SDK. From the repo folder:

```bash
dotnet run --project Tablerito
```

## Tests

```bash
mise run test
```

Without mise, `dotnet test`.
