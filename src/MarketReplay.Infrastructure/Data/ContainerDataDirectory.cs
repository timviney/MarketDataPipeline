using System.IO.Abstractions;
using MarketReplay.Core.Domain.Interfaces;

namespace MarketReplay.Infrastructure.Data;

public class ContainerDataDirectory : IDataDirectory
{
    private readonly FileSystem _fileSystem = new();

    public string DataPath()
    {
        return "data";
    }

    public IFileSystem FileSystem => _fileSystem;
}