using MarketReplay.Core.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MarketReplay.Infrastructure.Postgres;

public class PostgresResetter(IServiceScopeFactory scopeFactory) : IPersistenceResetter
{
    public async Task ClearAsync()
    {
        var scope = scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ITickCalcRepository>();
        await repo.ClearAsync();
    }
}