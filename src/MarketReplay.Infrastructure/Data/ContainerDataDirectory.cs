using System.IO.Abstractions;
using MarketReplay.Core.Domain.Interfaces;

namespace MarketReplay.Infrastructure.Data;

public class ContainerDataDirectory : IDataDirectory
{
    private readonly FileSystem _fileSystem = new();

    public string DataPath()
    {
        //TODO fix so this is testable locally and in the container!
        //return "C:\\Dev\\MarketDataPipeline\\data";
        return "data";
    }

    public IFileSystem FileSystem => _fileSystem;
}