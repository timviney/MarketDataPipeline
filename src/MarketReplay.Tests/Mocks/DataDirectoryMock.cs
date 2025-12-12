using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using MarketReplay.Core.Domain.Interfaces;

namespace MarketReplay.Tests.Mocks;

public class DataDirectoryMock : IDataDirectory
{
    private readonly MockFileSystem _mockFileSystem = new();

    public DataDirectoryMock()
    {
        _mockFileSystem.AddDirectory("data");
    }
    
    public string DataPath()
    {
        return _mockFileSystem.Path.Combine("data");
    }
    
    public IFileSystem FileSystem => _mockFileSystem;

    public void AddDirectory(params string[] pathSegments)
    {
        var fullPath = new List<string> { DataPath() }.Concat(pathSegments).ToArray();
        _mockFileSystem.AddDirectory(Path.Combine(fullPath));
    }

    public void AddFile(string fileData, string filename, params string[] pathSegments)
    {
        var fullPath = new List<string> { DataPath() };
        fullPath.AddRange(pathSegments);
        fullPath.Add(filename);
        
        _mockFileSystem.AddFile(Path.Combine(fullPath.ToArray()), new MockFileData(fileData));
    }
}