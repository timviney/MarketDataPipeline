using MarketReplay.Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MarketReplay.Api.Hubs;

public static class HubMappings
{
    public static void MapSignalRHubs(WebApplication webApplication)
    {
        webApplication.MapHub<NotificationHub>("/notificationHub").RequireAuthorization();
        webApplication.MapHub<ReplayHub>("/replayHub").RequireAuthorization();
        webApplication.MapHub<SymbolHub>("/symbolHub").RequireAuthorization();

        webApplication.MapPost("/sendNotification", async (string user, string message, IHubContext<NotificationHub, INotificationClient> hubContext) =>
            {
                await hubContext.Clients.All.ReceiveMessage(user, message);
                return Results.Ok();
            })
            .WithName("Test Notification")
            .WithTags("_Test");
    }
}