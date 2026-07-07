using Common.DataAccessService.Abstractions;
using Common.Platform.Domain.Abstractions;
using Microsoft.Data.SqlClient;

namespace Candidate.WebAPI.Middleware
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionHandlingMiddleware> _logger;

		public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception exception)
			{
				context.Response.ContentType = "application/json";

				Result<object> response;
				int statusCode;

				switch (exception)
				{
					case ValidationException validationEx:
						statusCode = StatusCodes.Status400BadRequest;
						response = Result.Failure<object>(HttpResponseStatusCodes.BadRequest, validationEx.Error.Fields is null ? Error.BadRequest(validationEx.Error.Message) : Error.BadRequest("Invalid information. Please check the highlighted fields.", validationEx.Error.Fields));
						break;

					case SqlException sqlEx:

						SqlDatabaseError error = sqlEx.Number switch
						{
							(int)SqlErrorNumber.ForeignKeyViolation => SqlDatabaseError.ForeignKeyViolation,
							(int)SqlErrorNumber.DeadlockVictim => SqlDatabaseError.DeadlockVictim,
							(int)SqlErrorNumber.UniqueIndexViolation => SqlDatabaseError.UniqueIndexViolation,
							(int)SqlErrorNumber.UniqueConstraintViolation => SqlDatabaseError.UniqueConstraintViolation,
							(int)SqlErrorNumber.CannotOpenDatabase => SqlDatabaseError.CannotOpenDatabase,
							(int)SqlErrorNumber.LoginFailed => SqlDatabaseError.LoginFailed,
							_ => SqlDatabaseError.Unknown
						};

						_logger.LogError(exception, "SQL exception occurred");

						statusCode = (int)error.HttpStatusCode;
						response = Result.Failure<object>(error.HttpStatusCode, error);

						break;

					case DirectoryNotFoundException ioEx:

						_logger.LogError(exception, "Path Not Found");
						statusCode = StatusCodes.Status404NotFound;
						response = Result.Failure<object>(HttpResponseStatusCodes.NotFound, Error.NotFound("Path of the file"));

						break;

					case FileNotFoundException ioEx:

						_logger.LogError(exception, "File Not Found");
						statusCode = StatusCodes.Status404NotFound;
						response = Result.Failure<object>(HttpResponseStatusCodes.NotFound, Error.NotFound("File or document"));

						break;

					default:
						statusCode = StatusCodes.Status500InternalServerError;
						_logger.LogError(exception, "Unhandled exception occurred.");
						response = Result.Failure<object>(HttpResponseStatusCodes.InternalServerError, Error.InternalServerError);
						break;
				}

				context.Response.StatusCode = statusCode;
				await context.Response.WriteAsJsonAsync(response);
			}
		}
	}
}
