namespace MarketReplay.Core.Domain.Model;

public record MarketTick(
    string Ticker,
    string Period,
    DateOnly Date,
    TimeOnly Time,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    long Volume
);