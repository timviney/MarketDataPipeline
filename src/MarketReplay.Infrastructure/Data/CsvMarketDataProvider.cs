using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Domain.Model;

namespace MarketReplay.Infrastructure.Data;

public class CsvMarketDataProvider(IDataDirectory dataDirectory) : IMarketDataProvider
{
    private const string Granularity = "5Min";
    
    public async Task<List<MarketTick>> LoadData()
    {
        var basePath = dataDirectory.DataPath();

        var results = new List<MarketTick>();
        
        foreach (var symbolFolder in dataDirectory.FileSystem.Directory.EnumerateDirectories(basePath))  
        {
            var symbol = Path.GetFileName(symbolFolder);
            foreach (var file in dataDirectory.FileSystem.Directory.EnumerateFiles(Path.Combine(symbolFolder, Granularity)))
            {
                using var csvReader = dataDirectory.FileSystem.File.OpenText(file);

                await csvReader.ReadLineAsync(); // skip headers
                while (!csvReader.EndOfStream)
                {
                    var line = await csvReader.ReadLineAsync();
                    var values = line!.Split(',');

                    var dateTime = values[0];
                    var open = values[1];
                    var high = values[2];
                    var low = values[3];
                    var close = values[4];
                    var vol = values[5];

                    var dateTimeValue = DateTime.Parse(dateTime);
                    var date = new DateOnly(dateTimeValue.Year, dateTimeValue.Month, dateTimeValue.Day);
                    var time = new TimeOnly(dateTimeValue.Hour, dateTimeValue.Minute, 0);

                    var tick = new MarketTick(symbol, Granularity, date, time, decimal.Parse(open),
                        decimal.Parse(high), decimal.Parse(low), decimal.Parse(close), (int)Math.Round(double.Parse(vol)));
                    
                    results.Add(tick);
                }
            }
        }
        
        return results;
    }
}