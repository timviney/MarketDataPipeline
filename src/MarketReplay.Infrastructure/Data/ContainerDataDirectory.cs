using System.IO.Abstractions;
using MarketReplay.Core.Domain.Interfaces;

namespace MarketReplay.Infrastructure.Data;

public class ContainerDataDirectory : IDataDirectory
{
    private readonly FileSystem _fileSystem = new();

    public string DataPath()
    {
        var currentDirectory = _fileSystem.Directory.GetCurrentDirectory();

        var solutionRoot = Path.GetFullPath(Path.Combine(currentDirectory, "../.."));

        var folder = currentDirectory.Contains("/app") ? "app/data" : "data";

        return Path.Combine(solutionRoot, folder);
    }

    public IFileSystem FileSystem => _fileSystem;
}