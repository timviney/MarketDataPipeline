using System.Collections.Immutable;
using AutoFixture;
using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Domain.Model;
using MarketReplay.Core.Services.Pipeline.Processors;
using NSubstitute;

namespace MarketReplay.Tests.Unit.Core.Domain.Services.Pipeline.Processors;

public class CalculationProcessorTests
{
    private CalculationProcessor _processor;
    private IMarketStateStore _marketStateStore;
    private ITickCalculationPublisher _tickCalculationPublisher;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _marketStateStore = Substitute.For<IMarketStateStore>();
        _tickCalculationPublisher = Substitute.For<ITickCalculationPublisher>();
        
        _processor = new CalculationProcessor(_marketStateStore, _tickCalculationPublisher);
    }

    [Test]
    public async Task GivenValidTickWithNoHistory_ThenCalculatesCorrectlyWithSmaEqualToTick()
    {
        //Arrange
        var tick = _fixture.Create<MarketTick>() with { Close = 123 };
        _marketStateStore.GetHistory(tick.Symbol).Returns(new Dictionary<DateTime, MarketTick>());
        
        //Act
        await _processor.ProcessAsync(tick);
        
        //Assert
        _marketStateStore.Received(1).GetHistory(tick.Symbol);
        _marketStateStore.Received(1).UpdateCalculations(Arg.Any<TickCalculations>());
        await _tickCalculationPublisher.PublishAsync(Arg.Is<TickCalculations>(x => x.Tick == tick && x.DailySma == 123));
    }


    [Test] public async Task GivenValidTickWithSmallHistory_ThenCalculatesCorrectly()
    {
        //Arrange
        var tick = _fixture.Create<MarketTick>();
        var history = _fixture.CreateMany<MarketTick>(10).ToDictionary(m => m.DateTime, m => m);
            
        _marketStateStore.GetHistory(tick.Symbol).Returns(history);

        TickCalculations recordedValue = null!;
        await _tickCalculationPublisher.PublishAsync(Arg.Do<TickCalculations>(x => recordedValue = x));
        
        //Act
        await _processor.ProcessAsync(tick);
        
        //Assert
        _marketStateStore.Received(1).GetHistory(tick.Symbol);
        _marketStateStore.Received(1).UpdateCalculations(Arg.Any<TickCalculations>());

        var expAvg = (history.Sum(p => p.Value.Close) + tick.Close)
                     / (history.Count+1);
        Assert.That(recordedValue.DailySma, Is.EqualTo(expAvg));
    }
    
    
    [Test] 
    public async Task GivenValidTickWithLargeHistory_ThenCalculatesCorrectly()
    {
        //Arrange
        var tick = _fixture.Create<MarketTick>();
        var history = _fixture.CreateMany<MarketTick>(12345).ToDictionary(m => m.DateTime, m => m);
            
        _marketStateStore.GetHistory(tick.Symbol).Returns(history);

        TickCalculations recordedValue = null!;
        await _tickCalculationPublisher.PublishAsync(Arg.Do<TickCalculations>(x => recordedValue = x));
        
        //Act
        await _processor.ProcessAsync(tick);
        
        //Assert
        _marketStateStore.Received(1).GetHistory(tick.Symbol);
        _marketStateStore.Received(1).UpdateCalculations(Arg.Any<TickCalculations>());

        var lastDay = history.ToImmutableSortedDictionary().Values.TakeLast(287);

        var expAvg = (lastDay.Sum(p => p.Close) + tick.Close)
                     / 288;
        Assert.That(recordedValue.DailySma, Is.EqualTo(expAvg));
    }

}