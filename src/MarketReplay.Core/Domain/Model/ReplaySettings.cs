namespace MarketReplay.Core.Domain.Model;

public sealed record ReplaySettings(
    decimal SpeedMultiplier,
    DateTime? StartAt = null,
    DateTime? EndAt = null
);
