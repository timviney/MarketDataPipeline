using MarketReplay.Core.Domain.Model;

namespace MarketReplay.Core.Domain.Interfaces;

public interface IEventProcessor
{
    Task ProcessAsync(MarketTick tick);
}