using Dapper;
using MarketReplay.Core.Domain.Interfaces;

namespace MarketReplay.Infrastructure.Postgres;

public class TickCalcRepository(IDbConnectionFactory factory) : ITickCalcRepository
{
    public async Task InsertAsync(TickCalcRow tick)
    {
        const string sql =
            """
            INSERT INTO tick_calculations 
                (symbol, timestamp, open, high, low, close, volume, daily_moving_average)
                VALUES (@Symbol, @Timestamp, @Open, @High, @Low, @Close, @Volume, @DailyMovingAverage)
            """;

        await using var con = factory.Create();

        await con.ExecuteAsync(sql, new
        {
            Symbol = tick.Symbol,
            Timestamp = tick.Timestamp,
            Open = tick.Open,
            High = tick.High,
            Low = tick.Low,
            Close = tick.Close,
            Volume = tick.Volume,
            DailyMovingAverage = tick.DailyMovingAverage
        });
    }

    public async Task ClearAsync()
    {
        const string sql =
            """
            TRUNCATE TABLE tick_calculations
            """;

        await using var con = factory.Create();
        await con.ExecuteAsync(sql);
    }
}