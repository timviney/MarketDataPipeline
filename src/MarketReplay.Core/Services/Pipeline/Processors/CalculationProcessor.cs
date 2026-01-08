using System.Collections.Immutable;
using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Domain.Model;

namespace MarketReplay.Core.Services.Pipeline.Processors;

public class CalculationProcessor(IMarketStateStore state, ITickCalculationPublisher publisher) : IEventProcessor
{
    public async Task ProcessAsync(MarketTick tick)
    {
        var symbol = tick.Symbol;
        
        var ticks = state.GetHistory(symbol);
        
        // daily SMA
        const int ticksToCalculate = 287; // 1 day in 5min ticks

        // This will be slow, would need to key by date in storage to optimise this better
        var slice = ticks.ToImmutableSortedDictionary().Values
            .TakeLast(ticksToCalculate).ToList();
        
        slice.Add(tick);

        var sum = slice.Sum(t => t.Close);
        
        var sma = sum / slice.Count;

        var tickCalculations = new TickCalculations(tick, sma);
        
        state.UpdateCalculations(tickCalculations);
        await publisher.PublishAsync(tickCalculations);
    }
}
