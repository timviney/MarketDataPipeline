using MarketReplay.Core.Domain.Model;
using MarketReplay.Infrastructure.Kafka;
using Microsoft.Extensions.Hosting;

namespace MarketReplay.Infrastructure.Postgres;

public class TickCalculationPersistenceConsumer(KafkaTopicCreator topicCreator) : KafkaConsumerBase<TickCalculations>(topicCreator)
{
    protected override string Topic => "tick-calculations";
    protected override Task HandleAsync(TickCalculations message)
    {
        //TODO: batch then write to DB
        return Task.CompletedTask;
    }
}