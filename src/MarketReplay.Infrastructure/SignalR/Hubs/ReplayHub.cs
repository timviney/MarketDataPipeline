using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MarketReplay.Infrastructure.SignalR.Hubs;

[Authorize]
public class ReplayHub : Hub<IReplayClient>
{
}