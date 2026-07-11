using Organization.Application.Abstractions.Files;
using Organization.WebAPI.Adapters;
using Microsoft.AspNetCore.Http;

namespace Organization.WebAPI.Extensions
{
    public static class FormFileExtensions
    {
        public static IIncomingFile AsIncoming(this IFormFile file) => new FormFileIncomingFile(file);
    }
}
