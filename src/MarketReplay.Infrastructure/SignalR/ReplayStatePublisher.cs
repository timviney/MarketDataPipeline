using MarketReplay.Core.Application;
using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MarketReplay.Infrastructure.SignalR;

public class ReplayStatePublisher(IHubContext<ReplayHub, IReplayClient> hubContext) : IReplayStatePublisher
{
    public async Task PublishAsync(ReplayState state)
    {
        await hubContext.Clients.All.GetState(state);
    }

    public async Task PublishClearedMessage()
    {
        await hubContext.Clients.All.HasBeenCleared();
    }
}