using MarketReplay.Infrastructure.Data;
using MarketReplay.Tests.Mocks;

namespace MarketReplay.Tests.Unit.Infrastructure.Data;

[TestFixture]
public class CsvMarketDataProviderTests
{
    private CsvMarketDataProvider _provider;
    private DataDirectoryMock _dataDirectory;

    [SetUp]
    public void SetUp()
    {
        // Set up the mock file system
        _dataDirectory = new DataDirectoryMock();
        _provider = new CsvMarketDataProvider(_dataDirectory);
    }

    [Test]
    public async Task LoadData_ShouldReturnEmpty_WhenNoDirectoriesExist()
    {
        // Arrange
        // no data added
        
        // Act
        var result = await _provider.LoadData();

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task LoadData_ShouldReturnEmpty_WhenNoFilesExistInDirectory()
    {
        // Arrange
        _dataDirectory.AddDirectory("Symbol1", "5Min");

        // Act
        var result = await _provider.LoadData();

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task LoadData_ShouldThrowFormatException_OnMalformedCsvData()
    {
        // Arrange
        
        var malformedCsv =
            "DateTime,Open,High,Low,Close,Volume\n" +
            "2023-01-01 10:00:00,InvalidData,50,45,48,100";

        _dataDirectory.AddFile(malformedCsv, "23-01.csv", "Symbol1", "5Min");

        // Act & Assert
        Assert.ThrowsAsync<FormatException>(async () => await _provider.LoadData());
    }

    [Test]
    public async Task LoadData_ShouldReturnResults_FromValidCsv()
    {
        // Arrange

        var validCsv =
            "DateTime,Open,High,Low,Close,Volume\n" +
            "2023-01-01 10:00:00,48.5,50,45,48,100";

        _dataDirectory.AddFile(validCsv, "23-01.csv", "Symbol1", "5Min");

        // Act
        var result = await _provider.LoadData();

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(1));
        var tick = result.First();
        Assert.That(tick.Symbol, Is.EqualTo("Symbol1"));
        Assert.That(tick.Period, Is.EqualTo("5Min"));
        Assert.That(tick.Date, Is.EqualTo(new DateOnly(2023, 1, 1)));
        Assert.That(tick.Time, Is.EqualTo(new TimeOnly(10, 0, 0)));
        Assert.That(tick.Open, Is.EqualTo(48.5m));
        Assert.That(tick.High, Is.EqualTo(50m));
        Assert.That(tick.Low, Is.EqualTo(45m));
        Assert.That(tick.Close, Is.EqualTo(48m));
        Assert.That(tick.Volume, Is.EqualTo(100));
    }
}