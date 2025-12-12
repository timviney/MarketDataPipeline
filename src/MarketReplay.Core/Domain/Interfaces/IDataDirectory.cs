using System.IO.Abstractions;

namespace MarketReplay.Core.Domain.Interfaces;

public interface IDataDirectory
{
    string DataPath();
    
    IFileSystem FileSystem { get; }
}