using Common.Platform.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Job.Application.Abstractions.Behaviors
{
	public class RequestLoggingBehavior<TRequest, TResponse>
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : class
	where TResponse : Result
	{
		private readonly ILogger<RequestLoggingBehavior<TRequest, TResponse>> _logger;

		public RequestLoggingBehavior(ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger)
		{
			_logger = logger;
		}

		public async Task<TResponse> Handle(
			TRequest request,
			RequestHandlerDelegate<TResponse> next,
			CancellationToken cancellationToken)
		{
			string requestName = typeof(TRequest).Name;

			_logger.LogInformation("Processing request {RequestName}", requestName);

			TResponse result = await next();

			if (result.IsSuccess)
			{
				_logger.LogInformation("Completed request {RequestName}", requestName);
			}
			else
			{
				_logger.LogError("Completed request {RequestName} with error: {Error}", requestName, result.Error);
			}

			return result;
		}
	}
}
