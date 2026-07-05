using System.IO;

namespace Candidate.Application.Abstractions.Files
{
    public interface IIncomingFile
    {
        string FileName { get; }
        long Length { get; }
        Stream OpenReadStream();
    }
}
