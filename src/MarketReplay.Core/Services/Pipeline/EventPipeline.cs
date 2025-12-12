using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Domain.Model;

namespace MarketReplay.Core.Services.Pipeline;

public class EventPipeline(IEventProcessor[] processors) : IEventPipeline
{
    public async Task PublishAsync(MarketTick tick)
    {
        foreach (var processor in processors)
        {
            await processor.ProcessAsync(tick);
        }
    }
}