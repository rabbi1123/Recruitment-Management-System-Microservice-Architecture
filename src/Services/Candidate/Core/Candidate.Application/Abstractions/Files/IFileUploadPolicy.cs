using System.Collections.Generic;

namespace Candidate.Application.Abstractions.Files
{
    public interface IFileUploadPolicy
    {
        string Root { get; }
        long MaxBytes { get; }

        List<string> AllowedExtensions { get; }
    }
}
