using Job.Application.Abstractions.Files;
using Common.Platform.Domain.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Job.Application.Abstractions.Images
{
    public interface IImageValidator
    {
        Task<Result<BufferedImage>> ValidateAndBufferAsync(IIncomingFile file, CancellationToken ct);
    }
}
