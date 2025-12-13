using System.Collections.Concurrent;
using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Domain.Model;

namespace MarketReplay.Infrastructure.State;

public class InMemoryMarketStateStore : IMarketStateStore
{
    private readonly ConcurrentDictionary<string, MarketTick> _latest = new();
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<DateTime, MarketTick>> _history = new();
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<DateTime, TickCalculations>> _calculationHistory = new();
    public void UpdateLatestTick(MarketTick tick)
    {
        _latest[tick.Symbol] = tick;
        
        var symbolHistory = _history.GetOrAdd(
            key: tick.Symbol,
            valueFactory: (symbol) => new ConcurrentDictionary<DateTime, MarketTick>()
        );
        
        symbolHistory[tick.DateTime] = tick;
    }

    public void UpdateCalculations(TickCalculations calculations)
    {
        var symbolHistory = _calculationHistory.GetOrAdd(
            key: calculations.Tick.Symbol,
            valueFactory: (symbol) => new ConcurrentDictionary<DateTime, TickCalculations>()
        );
        
        symbolHistory[calculations.Tick.DateTime] = calculations;
    }

    public Dictionary<DateTime, MarketTick> GetHistory(string symbol) => _history.GetValueOrDefault(symbol)?.ToDictionary() 
                                                                         ?? new Dictionary<DateTime, MarketTick>();
    public Dictionary<DateTime, TickCalculations> GetCalculationHistory(string symbol) => _calculationHistory.GetValueOrDefault(symbol)?.ToDictionary() 
                                                                         ?? new Dictionary<DateTime, TickCalculations>();
    public MarketTick? GetLatest(string symbol) => _latest.GetValueOrDefault(symbol);
    public List<string> GetSymbols() => _history.Keys.ToList();
}