using Recruitment.WebAPI.Middleware;

namespace Recruitment.WebAPI.Extensions
{
	public static class MiddlewareExtensions
	{
		public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
		{
			return app.UseMiddleware<ExceptionHandlingMiddleware>();
		}
	}
}
