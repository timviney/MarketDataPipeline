using System.Text.Json;
using Confluent.Kafka;
using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Domain.Model;

namespace MarketReplay.Infrastructure.Kafka;

public class KafkaTickCalculationPublisher(IProducer<string, string> producer) : ITickCalculationPublisher
{
    private const string Topic = "tick-calculations";
    public async Task PublishAsync(TickCalculations calculation)
    {
        var json = JsonSerializer.Serialize(calculation);
        
        await producer.ProduceAsync(
            Topic, 
            new Message<string, string>
            {
                Key = calculation.Tick.Symbol,
                Value = json
            });
    }
}