using Candidate.Application.Abstractions.Files;
using Candidate.WebAPI.Adapters;
using Microsoft.AspNetCore.Http;

namespace Candidate.WebAPI.Extensions
{
    public static class FormFileExtensions
    {
        public static IIncomingFile AsIncoming(this IFormFile file) => new FormFileIncomingFile(file);
    }
}
