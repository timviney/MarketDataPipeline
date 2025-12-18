using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Domain.Model;
using MarketReplay.Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MarketReplay.Infrastructure.SignalR;

// Depreciated! Now consumes from Kafka instead
public class SignalRTickCalculationPublisher(IHubContext<SymbolHub, ISymbolClient> hubContext) : ITickCalculationPublisher
{
    public async Task PublishAsync(TickCalculations calculation)
    {
        await hubContext.Clients.All.LatestCalculation(calculation.Tick.Symbol, calculation);
    }
}