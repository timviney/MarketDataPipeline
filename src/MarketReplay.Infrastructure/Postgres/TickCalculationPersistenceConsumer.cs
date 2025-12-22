using MarketReplay.Core.Domain.Model;
using MarketReplay.Infrastructure.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MarketReplay.Infrastructure.Postgres;

public class TickCalculationPersistenceConsumer(KafkaTopicCreator topicCreator, IServiceScopeFactory scopeFactory, TickCalcMapper map) : KafkaConsumerBase<TickCalculations>(topicCreator)
{
    protected override string Topic => "tick-calculations";
    protected override async Task HandleAsync(TickCalculations message)
    {
        // TODO if this becomes too heavy we can start batching here :)
        var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITickCalcRepository>();
        
        await repository.InsertAsync(map.ToRow(message));
    }
}