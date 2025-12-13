using MarketReplay.Core.Domain.Model;

namespace MarketReplay.Core.Domain.Interfaces;

public interface IMarketStateStore
{
    void Clear();
    void UpdateLatestTick(MarketTick tick);
    void UpdateCalculations(TickCalculations calculations);
    Dictionary<DateTime, MarketTick> GetHistory(string symbol);
    Dictionary<DateTime, TickCalculations> GetCalculationHistory(string symbol);

    MarketTick? GetLatest(string symbol);
    List<string> GetSymbols();
}