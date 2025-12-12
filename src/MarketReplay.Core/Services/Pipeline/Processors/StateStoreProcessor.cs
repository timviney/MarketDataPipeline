using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Domain.Model;

namespace MarketReplay.Core.Services.Pipeline.Processors;

public class StateStoreProcessor : IEventProcessor
{
    private readonly IMarketStateStore _state;

    public StateStoreProcessor(IMarketStateStore state)
    {
        _state = state;
    }

    public Task ProcessAsync(MarketTick tick)
    {
        _state.UpdateLatestTick(tick);
        return Task.CompletedTask;
    }
}
