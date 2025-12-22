namespace MarketReplay.Infrastructure.Postgres;

public interface ITickCalcRepository
{
    Task InsertAsync(TickCalcRow tick);
    Task ClearAsync();
}