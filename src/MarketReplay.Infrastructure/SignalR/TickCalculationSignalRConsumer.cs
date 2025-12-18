using MarketReplay.Core.Domain.Model;
using MarketReplay.Infrastructure.Kafka;
using MarketReplay.Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MarketReplay.Infrastructure.SignalR;

public class TickCalculationSignalRConsumer(IHubContext<SymbolHub, ISymbolClient> hubContext, KafkaTopicCreator topicCreator) 
    : KafkaConsumerBase<TickCalculations>(topicCreator)
{
    protected override string Topic => "tick-calculations";
    protected override async Task HandleAsync(TickCalculations calculation)
    {
        await hubContext.Clients.All.LatestCalculation(calculation.Tick.Symbol, calculation);
    }
}