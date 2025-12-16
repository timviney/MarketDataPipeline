using MarketReplay.Core.Domain.Model;

namespace MarketReplay.Core.Domain.Interfaces;

public interface ITickCalculationPublisher
{
    Task PublishAsync(TickCalculations calculation);
}

