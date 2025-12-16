namespace MarketReplay.Infrastructure.SignalR.Hubs;

public interface INotificationClient
{
    Task ReceiveMessage(string user, string message);
}