using MarketReplay.Core.Domain.Model;

namespace MarketReplay.Infrastructure.Postgres;

public sealed record TickCalcRow
(
    string Symbol,
    DateTime Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume,
    decimal DailyMovingAverage
)
{

}
