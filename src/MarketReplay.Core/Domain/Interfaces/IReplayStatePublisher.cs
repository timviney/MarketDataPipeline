using MarketReplay.Core.Application;

namespace MarketReplay.Core.Domain.Interfaces;

public interface IReplayStatePublisher
{
    Task PublishAsync(ReplayState state);
    Task PublishClearedMessage();
}