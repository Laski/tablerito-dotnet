using Tablerito.Models;

namespace Tablerito.Services;

public class NotificationService
{
    private readonly NotificationClient notifications;
    private readonly string environment;
    private readonly ILogger<NotificationService> logger;

    public NotificationService(NotificationClient notifications, IConfiguration config, ILogger<NotificationService> logger)
    {
        this.notifications = notifications;
        this.environment = config["Tablerito:Environment"]!;
        this.logger = logger;
    }

    public void Notify(Notification notification)
    {
        if (environment == "production") notifications.Send(notification);
        else logger.LogInformation("Simulated notification: {Notification}", notification);
    }
}
