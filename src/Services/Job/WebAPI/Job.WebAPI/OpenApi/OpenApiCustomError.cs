using Common.Platform.Domain.Abstractions;

namespace Job.WebAPI.OpenApi
{
    public class OpenApiCustomError : Error
    {
        public OpenApiCustomError() : base(string.Empty, null, null)
        {
        }
    }
}
