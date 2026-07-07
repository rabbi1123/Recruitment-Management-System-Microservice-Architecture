using Candidate.Application.Abstractions.Validation;
using Common.Platform.Domain.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using ValidationException = Common.Platform.Domain.Abstractions.ValidationException;

namespace Candidate.Application.Abstractions.Behaviors
{
	public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
		where TResponse : Result
	{
		private readonly IRequestValidator<TRequest> _dataAnnotationValidator;
		private readonly IEnumerable<IValidator<TRequest>> _fluentValidators;
		private readonly ILogger<RequestValidationBehavior<TRequest, TResponse>> _logger;

		public RequestValidationBehavior(
			IRequestValidator<TRequest> dataAnnotationValidator,
			ILogger<RequestValidationBehavior<TRequest, TResponse>> logger,
			IEnumerable<IValidator<TRequest>> fluentValidators)
		{
			_dataAnnotationValidator = dataAnnotationValidator;
			_logger = logger;
			_fluentValidators = fluentValidators;
		}

		public async Task<TResponse> Handle(
			TRequest request,
			RequestHandlerDelegate<TResponse> next,
			CancellationToken cancellationToken)
		{
			// 1. Data annotation validation
			var validationResult = _dataAnnotationValidator.Validate(request);

			if (validationResult.IsFailure)
			{
				_logger.LogWarning("Data annotation Validation failed for request {RequestType}", typeof(TRequest).Name);

				throw new ValidationException(validationResult.Error);
			}

			// 2. FluentValidation (if any validators exist)
			if (_fluentValidators.Any())
			{
				var context = new ValidationContext<TRequest>(request);
				var fluentValidationResults = await Task.WhenAll(
					_fluentValidators.Select(v => v.ValidateAsync(context, cancellationToken))
				);

				var failures = fluentValidationResults
					.SelectMany(r => r.Errors)
					.Where(f => f != null)
					.ToList();

				if (failures.Count > 0)
				{
					var errors = failures
								.GroupBy(f => CamelizePath(f.PropertyName))     // <= key fix
								.ToDictionary(g => g.Key, g => g.First().ErrorMessage);

					_logger.LogWarning("Fluent validation failed for {RequestType}", typeof(TRequest).Name);

					var fluentValidationResult = Result.Failure(HttpResponseStatusCodes.BadRequest, Error.BadRequest("Invalid information. Please check the highlighted fields.", errors));

					throw new ValidationException(fluentValidationResult.Error);
				}
			}

			return await next();
		}


		private static string CamelizePath(string? path)
		{
			if (string.IsNullOrWhiteSpace(path))
				return string.Empty;

			// Split on dots, camelCase each segment's name part, keep any [0][1] indexers
			// Examples:
			// "PersonalIdentifications[1].ExpiryDate" -> "personalIdentifications[1].expiryDate"
			// "Documents[0].Files[2].FileName"        -> "documents[0].files[2].fileName"
			var segments = path.Split('.');

			for (int i = 0; i < segments.Length; i++)
			{
				var seg = segments[i];
				// Split the segment into "name" and trailing "[indexes]" (if any)
				int bracketStart = seg.IndexOf('[');
				string name = bracketStart >= 0 ? seg[..bracketStart] : seg;
				string indexes = bracketStart >= 0 ? seg[bracketStart..] : string.Empty;

				if (!string.IsNullOrEmpty(name))
				{
					// basic camelCase: lower first char only
					// (keeps acronyms like "URL" -> "uRL"; adjust if you want smarter casing)
					name = char.ToLowerInvariant(name[0]) + name.Substring(1);
				}

				segments[i] = name + indexes;
			}

			return string.Join('.', segments);
		}
	}
}
