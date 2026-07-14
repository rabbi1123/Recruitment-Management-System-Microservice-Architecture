using System.IO;

namespace Job.Application.Abstractions.Files
{
    public interface IIncomingFile
    {
        string FileName { get; }
        long Length { get; }
        Stream OpenReadStream();
    }
}
