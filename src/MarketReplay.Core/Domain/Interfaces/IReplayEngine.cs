namespace MarketReplay.Core.Domain.Interfaces;

public interface IReplayEngine
{
    Task StartAsync(int speed = 1);
    Task PauseAsync();
    Task StopAsync();
    string GetStatus();
}
