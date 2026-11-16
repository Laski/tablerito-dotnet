using NLog.Web;
using Tablerito.Services;

var builder = WebApplication.CreateBuilder(args);

// development or production
var environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "development";
if (environment == "production")
    builder.Configuration.AddJsonFile("settings.production.json");
else
    builder.Configuration.AddJsonFile("settings.development.json");

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<BoardService>();
builder.Services.AddSingleton<AttachmentService>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddSingleton<NotificationClient>();

builder.Services.Configure<HostOptions>(o => o.ShutdownTimeout = TimeSpan.Zero);

var app = builder.Build();

app.Services.GetRequiredService<BoardService>().SeedSampleBoard();

app.Services.GetRequiredService<NotificationClient>().WaitUntilAvailable();

app.UseStaticFiles();
app.MapControllers();

app.Run();
