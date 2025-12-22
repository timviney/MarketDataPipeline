using Npgsql;

namespace MarketReplay.Infrastructure.Postgres;

public class PostgresConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public NpgsqlConnection Create()
    {
        return new NpgsqlConnection(connectionString);
    }
}