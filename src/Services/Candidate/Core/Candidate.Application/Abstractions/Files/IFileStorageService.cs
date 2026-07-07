using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Candidate.Application.Abstractions.Files
{
    public interface IFileStorageService
    {
        StoredFileInfo GenerateRelativePath(string subFolder, IIncomingFile file, string? newFileName);
        Task<StoredFileInfo> SaveAsync(IIncomingFile file, string relativePath, CancellationToken ct);
        Task DeleteAsync(string relativeFilePath, CancellationToken ct);
        Task<Stream> OpenReadAsync(string relativePath, CancellationToken ct);
        string GetMime(string? fileNameOrExt);
    }
}
