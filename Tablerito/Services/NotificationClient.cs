using System.Net.Http.Headers;
using Tablerito.Models;

namespace Tablerito.Services;

public class NotificationClient
{
    private readonly HttpClient http = new();
    private readonly string url;
    private readonly string apiKey;
    private readonly ILogger<NotificationClient> logger;

    public NotificationClient(IConfiguration config, ILogger<NotificationClient> logger)
    {
        url = config["Tablerito:Notifications:Url"]!;
        apiKey = config["Tablerito:Notifications:ApiKey"]!;
        this.logger = logger;
    }

    // Makes sure the notification service is up before taking requests
    public void WaitUntilAvailable(int attempts = 5)
    {
        for (var attempt = 1; attempt <= attempts; attempt++)
        {
            try
            {
                http.Send(new HttpRequestMessage(HttpMethod.Get, url));
                return;
            }
            catch (HttpRequestException)
            {
                logger.LogWarning("Notification service not available (attempt {Attempt} of {Attempts})", attempt, attempts);
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }
        logger.LogWarning("Starting without the notification service");
    }

    public void Send(Notification notification)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = JsonContent.Create(notification);
        http.Send(request).EnsureSuccessStatusCode();
    }
}
