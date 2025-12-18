using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;

namespace MarketReplay.Infrastructure.Kafka;

public abstract class KafkaConsumerBase<T>(KafkaTopicCreator topicCreator) : BackgroundService
{
    protected abstract string Topic { get; }
    protected abstract Task HandleAsync(T message);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await topicCreator.CreateTopicIfNecessary();
        
        var config = new ConsumerConfig
        {
            BootstrapServers = "kafka:9092",
            GroupId = GetType().Name,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        
        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(Topic);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);
            var message = JsonSerializer.Deserialize<T>(consumeResult.Message.Value)!;
            await HandleAsync(message);
        }
    }
}