namespace MarketReplay.Core.Domain.Model;

public record MarketTick(
    string Symbol,
    string Period,
    DateTime DateTime,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    long Volume
);