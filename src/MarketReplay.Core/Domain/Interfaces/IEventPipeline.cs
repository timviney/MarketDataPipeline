using MarketReplay.Core.Domain.Model;

namespace MarketReplay.Core.Domain.Interfaces;

public interface IEventPipeline
{
    Task PublishAsync(MarketTick tick);
}