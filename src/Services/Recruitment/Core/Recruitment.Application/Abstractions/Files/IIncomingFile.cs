using System.IO;

namespace Recruitment.Application.Abstractions.Files
{
    public interface IIncomingFile
    {
        string FileName { get; }
        long Length { get; }
        Stream OpenReadStream();
    }
}
