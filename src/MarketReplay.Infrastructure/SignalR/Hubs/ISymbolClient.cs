using MarketReplay.Core.Domain.Model;

namespace MarketReplay.Infrastructure.SignalR.Hubs;

public interface ISymbolClient
{
    Task LatestCalculation(string symbol, TickCalculations calculations);
}