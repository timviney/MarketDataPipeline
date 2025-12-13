using AutoFixture;
using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Domain.Model;
using MarketReplay.Core.Services.Replay;
using NSubstitute;

namespace MarketReplay.Tests.Unit.Core.Domain.Services.Replay;

[TestFixture]
public class ReplayEngineTests
{
    private ReplayEngine _engine;
    private IMarketDataProvider? _dataProvider;
    private IEventPipeline _eventPipeline;
    private Fixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _dataProvider = Substitute.For<IMarketDataProvider>();
        _eventPipeline = Substitute.For<IEventPipeline>();
        _engine = new ReplayEngine(_dataProvider, _eventPipeline);
    }
    
    [Test]
    public async Task StartAsync_ShouldLoadDataFromProvider()
    {
        await _engine.StartAsync();
        
        await _dataProvider.Received(1)!.LoadData();
    }

    [Test]
    public async Task StepAsync_WhenHasData_ShouldPublishCorrectNumberOfTicksToPipeline()
    {
        _dataProvider!.LoadData().ReturnsForAnyArgs(_fixture.CreateMany<MarketTick>(4).ToList());
        
        await _engine.StartAsync();
        
        await _engine.StepAsync();
        await _engine.StepAsync();
        await _engine.StepAsync();

        await _eventPipeline.Received(3).PublishAsync(Arg.Any<MarketTick>());
        
        var lastResult = await _engine.StepAsync();
        var repeatedResult = await _engine.StepAsync();
        
        Assert.That(lastResult, Is.True);
        Assert.That(repeatedResult, Is.False);
    }
    
    [Test]
    public async Task StepAsync_WhenNotStarted_ShouldNotPublishTickToPipeline()
    {
        _dataProvider!.LoadData().ReturnsForAnyArgs(_fixture.CreateMany<MarketTick>(10).ToList());
        
        var result = await _engine.StepAsync();

        await _eventPipeline.Received(0).PublishAsync(Arg.Any<MarketTick>());
        Assert.That(result, Is.False);
    }
}