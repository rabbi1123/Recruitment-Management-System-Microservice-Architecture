using Job.WebAPI.Middleware;

namespace Job.WebAPI.Extensions
{
	public static class MiddlewareExtensions
	{
		public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
		{
			return app.UseMiddleware<ExceptionHandlingMiddleware>();
		}
	}
}
