using Candidate.WebAPI.Middleware;

namespace Candidate.WebAPI.Extensions
{
	public static class MiddlewareExtensions
	{
		public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
		{
			return app.UseMiddleware<ExceptionHandlingMiddleware>();
		}
	}
}
