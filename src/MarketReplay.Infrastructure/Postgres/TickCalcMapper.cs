using MarketReplay.Core.Domain.Model;

namespace MarketReplay.Infrastructure.Postgres;

public class TickCalcMapper
{
    public TickCalcRow ToRow(TickCalculations calc)
    {
        return new TickCalcRow(
            Symbol: calc.Tick.Symbol,
            Timestamp: calc.Tick.DateTime,
            Open: calc.Tick.Open,
            High: calc.Tick.High,
            Low: calc.Tick.Low,
            Close: calc.Tick.Close,
            Volume: calc.Tick.Volume,
            DailyMovingAverage: calc.DailySma
        );
    }
}