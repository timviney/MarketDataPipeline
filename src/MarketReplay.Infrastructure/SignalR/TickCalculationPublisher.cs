using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Domain.Model;
using MarketReplay.Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MarketReplay.Infrastructure.SignalR;

public class TickCalculationPublisher(IHubContext<SymbolHub, ISymbolClient> hubContext) : ITickCalculationPublisher
{
    public async Task PublishAsync(TickCalculations calculation)
    {
        await hubContext.Clients.All.LatestCalculation(calculation.Tick.Symbol, calculation);
    }
}