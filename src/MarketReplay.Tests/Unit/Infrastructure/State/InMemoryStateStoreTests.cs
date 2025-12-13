using AutoFixture;
using MarketReplay.Core.Domain.Model;
using MarketReplay.Infrastructure.State;

namespace MarketReplay.Tests.Unit.Infrastructure.State
{
    [TestFixture]
    public class InMemoryMarketStateStoreTests
    {
        private InMemoryMarketStateStore _store;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _store = new InMemoryMarketStateStore();
            _fixture = new Fixture();
        }

        [Test]
        public void UpdateLatestTick_ShouldUpdateLatestAndHistoryStore()
        {
            // Arrange
            var tick = _fixture.Create<MarketTick>();

            // Act
            _store.UpdateLatestTick(tick);

            // Assert
            var latest = _store.GetLatest(tick.Symbol);
            Assert.That(latest, Is.Not.Null);
            Assert.That(latest, Is.EqualTo(tick));

            var history = _store.GetHistory(tick.Symbol);
            Assert.That(history.ContainsKey(tick.DateTime), Is.True);
            Assert.That(history[tick.DateTime], Is.EqualTo(tick));
        }

        [Test]
        public void UpdateCalculations_ShouldStoreCalculationHistory()
        {
            // Arrange
            var tick = _fixture.Create<MarketTick>();

            var calculations = _fixture.Build<TickCalculations>()
                .With(c => c.Tick, tick)
                .Create();

            // Act
            _store.UpdateCalculations(calculations);

            // Assert

            var history = _store.GetCalculationHistory(tick.Symbol);
            Assert.That(history.ContainsKey(tick.DateTime), Is.True);
            Assert.That(history[tick.DateTime], Is.EqualTo(calculations));
        }

        [Test]
        public void GetHistory_ShouldReturnEmptyDictWhenSymbolNotExists()
        {
            // Act
            var history = _store.GetHistory("GOOG");

            // Assert
            Assert.That(history.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetLatest_ShouldReturnNullForUnknownSymbol()
        {
            // Act
            var latest = _store.GetLatest("GOOG");

            // Assert
            Assert.That(latest, Is.Null);
        }

        [Test]
        public void GetSymbols_ShouldReturnAllTrackedSymbols()
        {
            // Arrange
            _store.UpdateLatestTick(_fixture.Create<MarketTick>() with { Symbol = "AAPL" });
            _store.UpdateLatestTick(_fixture.Create<MarketTick>() with { Symbol = "GOOG" });

            // Act
            var symbols = _store.GetSymbols();

            // Assert
            Assert.That(symbols.Count, Is.EqualTo(2));
            Assert.That(symbols.Contains("AAPL"), Is.True);
            Assert.That(symbols.Contains("GOOG"), Is.True);
        }
    }
}