using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Domain.Model;

namespace MarketReplay.Core.Services.Replay;

public class ReplayEngine(IMarketDataProvider dataProvider, IEventPipeline pipeline, IMarketStateStore stateStore, IPersistenceResetter resetter) : IReplayEngine
{
    private int _step;
    private List<MarketTick>? _data;

    public async Task StartAsync()
    {
        _data = await dataProvider.LoadData();
        _step = 0;
        stateStore.Clear();
        await resetter.ClearAsync();
    }

    public async Task<bool> StepAsync()
    {
        // Pace is handled outside of Core in the BackgroundService
        if (_data is null || _data.Count <= _step) return false;
        await pipeline.PublishAsync(_data[_step]);
        _step++;
        return true;
    }
}