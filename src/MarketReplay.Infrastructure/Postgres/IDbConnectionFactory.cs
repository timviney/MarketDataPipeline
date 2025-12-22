using Npgsql;

namespace MarketReplay.Infrastructure.Postgres;

public interface IDbConnectionFactory
{
    NpgsqlConnection Create();
}