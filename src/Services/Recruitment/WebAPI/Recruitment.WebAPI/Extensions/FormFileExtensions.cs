using Recruitment.Application.Abstractions.Files;
using Recruitment.WebAPI.Adapters;
using Microsoft.AspNetCore.Http;

namespace Recruitment.WebAPI.Extensions
{
    public static class FormFileExtensions
    {
        public static IIncomingFile AsIncoming(this IFormFile file) => new FormFileIncomingFile(file);
    }
}
