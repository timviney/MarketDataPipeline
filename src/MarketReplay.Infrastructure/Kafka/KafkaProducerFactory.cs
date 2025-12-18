using Confluent.Kafka;

namespace MarketReplay.Infrastructure.Kafka;

public class KafkaProducerFactory
{
    public static IProducer<string, string> Create(string bootstrapServers)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All
        };
        
        return new ProducerBuilder<string, string>(config).Build();
    }
}