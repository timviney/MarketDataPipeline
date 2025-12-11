namespace MarketReplay.Core.Domain.Model;

public sealed record Anomaly(
    string Symbol,
    DateTime Timestamp,
    string Type,
    string Description,
    double Severity //TODO decide what this really means lol
);
