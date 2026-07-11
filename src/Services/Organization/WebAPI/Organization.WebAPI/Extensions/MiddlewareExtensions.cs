using Organization.WebAPI.Middleware;

namespace Organization.WebAPI.Extensions
{
	public static class MiddlewareExtensions
	{
		public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
		{
			return app.UseMiddleware<ExceptionHandlingMiddleware>();
		}
	}
}
