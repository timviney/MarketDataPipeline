namespace MarketReplay.Core.Domain.Model;

public record MarketTick(
    string Symbol,
    string Period,
    DateOnly Date,
    TimeOnly Time,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    long Volume
);