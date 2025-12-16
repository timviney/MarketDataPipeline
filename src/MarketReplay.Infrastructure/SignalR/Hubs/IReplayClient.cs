using MarketReplay.Core.Application;

namespace MarketReplay.Infrastructure.SignalR.Hubs;

public interface IReplayClient
{
    Task GetState(ReplayState state);
}