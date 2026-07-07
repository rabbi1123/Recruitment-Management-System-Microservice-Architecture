using Common.Platform.Domain.Abstractions;

namespace Candidate.WebAPI.OpenApi
{
    public class OpenApiCustomError : Error
    {
        public OpenApiCustomError() : base(string.Empty, null, null)
        {
        }
    }
}
