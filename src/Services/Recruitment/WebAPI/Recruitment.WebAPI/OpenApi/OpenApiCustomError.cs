using Common.Platform.Domain.Abstractions;

namespace Recruitment.WebAPI.OpenApi
{
    public class OpenApiCustomError : Error
    {
        public OpenApiCustomError() : base(string.Empty, null, null)
        {
        }
    }
}
