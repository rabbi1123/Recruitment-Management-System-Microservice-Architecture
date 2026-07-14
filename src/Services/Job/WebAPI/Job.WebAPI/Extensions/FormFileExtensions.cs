using Job.Application.Abstractions.Files;
using Job.WebAPI.Adapters;
using Microsoft.AspNetCore.Http;

namespace Job.WebAPI.Extensions
{
    public static class FormFileExtensions
    {
        public static IIncomingFile AsIncoming(this IFormFile file) => new FormFileIncomingFile(file);
    }
}
