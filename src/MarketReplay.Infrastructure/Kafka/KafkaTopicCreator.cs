using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace MarketReplay.Infrastructure.Kafka;

public class KafkaTopicCreator
{
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    private bool _created;

    public async Task CreateTopicIfNecessary()
    {
        if (_created) return;
        
        await _semaphoreSlim.WaitAsync();
        
        try
        {
            if (_created) return;
            
            var adminConfig = new AdminClientConfig { BootstrapServers = "kafka:9092" };
            using var admin = new AdminClientBuilder(adminConfig).Build();
            
            await admin.CreateTopicsAsync(new[]
            {
                new TopicSpecification
                {
                    Name = "tick-calculations",
                    NumPartitions = 1,
                    ReplicationFactor = 1
                }
            });
            
            _created = true;
        }
        catch (CreateTopicsException e)
        {
            if (e.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                throw;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}