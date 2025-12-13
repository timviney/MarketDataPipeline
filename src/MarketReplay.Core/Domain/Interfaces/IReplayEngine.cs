namespace MarketReplay.Core.Domain.Interfaces;

public interface IReplayEngine
{
    Task StartAsync();
    Task<bool> StepAsync();
}
