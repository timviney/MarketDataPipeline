namespace MarketReplay.Core.Domain.Model;

public sealed class SymbolState
{
    public MarketTick? LatestTick { get; private set; }

    public decimal? MovingAverage10 { get; private set; }
    public decimal? MovingAverage20 { get; private set; }

    public double? Volatility { get; private set; }
    public long Volume1Min { get; private set; }

    private readonly Queue<decimal> _ma10 = new();
    private readonly Queue<decimal> _ma20 = new();
    private readonly Queue<decimal> _volWindow = new();
    
    public List<Anomaly> RecentAnomalies { get; } = new();

    public void Update(MarketTick tick)
    {
        LatestTick = tick;
    }
}
