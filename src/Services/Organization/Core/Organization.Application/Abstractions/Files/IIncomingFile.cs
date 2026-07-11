using System.IO;

namespace Organization.Application.Abstractions.Files
{
    public interface IIncomingFile
    {
        string FileName { get; }
        long Length { get; }
        Stream OpenReadStream();
    }
}
