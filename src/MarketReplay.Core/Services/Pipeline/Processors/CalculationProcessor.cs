using System.Collections.Immutable;
using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Domain.Model;

namespace MarketReplay.Core.Services.Pipeline.Processors;

public class CalculationProcessor(IMarketStateStore state) : IEventProcessor
{
    public Task ProcessAsync(MarketTick tick)
    {
        var symbol = tick.Symbol;
        
        var ticks = state.GetHistory(symbol);
        
        // daily SMA
        const int ticksToCalculate = 288; // 1 day in 5min ticks
        decimal sma = 0;
        if (ticks.Count < ticksToCalculate)
        {
            // This will be slow, would need to key by date in storage to optimise this better
            var slice = ticks.ToImmutableSortedDictionary()
                .TakeLast(ticksToCalculate);

            var sum = slice.Sum(p => p.Value.Close);
        
            sma = sum / ticksToCalculate;
        }
        
        state.UpdateCalculations(new TickCalculations(tick, sma));
        
        return Task.CompletedTask;
    }
}
