namespace MarketReplay.Core.Domain.Interfaces;

public interface IPersistenceResetter
{
    public Task ClearAsync();
}