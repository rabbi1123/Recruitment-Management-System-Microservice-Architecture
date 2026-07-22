using Common.Platform.Domain.Abstractions;
using Microsoft.Extensions.Logging;
using Recruitment.Application.Abstractions.Validation;
using System.ComponentModel.DataAnnotations;

namespace Recruitment.Application.Common.Validation
{
	public class RequestValidator<TRequest> : IRequestValidator<TRequest>
	{
		private readonly ILogger<RequestValidator<TRequest>> _logger;

		public RequestValidator(ILogger<RequestValidator<TRequest>> logger)
		{
			_logger = logger;
		}
		public Result Validate(TRequest request)
		{
			var validationResults = new List<ValidationResult>();
			var context = new ValidationContext(request);

			bool isValid = Validator.TryValidateObject(request, context, validationResults, validateAllProperties: true);


			if (!isValid)
			{
				var fieldErrors = new Dictionary<string, string>();

				foreach (var validationResult in validationResults)
				{
					// If no member names are specified, treat it as a general error
					if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
					{
						foreach (var memberName in validationResult.MemberNames)
						{
							var camelCaseName = ToCamelCase(memberName);

							var message = (validationResult.ErrorMessage ?? "Invalid value").Replace(memberName, camelCaseName);

							if (!fieldErrors.ContainsKey(camelCaseName))
							{
								fieldErrors[camelCaseName] = message ?? "Invalid value";
							}

							// Don't overwrite if the same field has multiple messages
							//if (!fieldErrors.ContainsKey(memberName))
							//{
							//    fieldErrors[memberName] = validationResult.ErrorMessage ?? "Invalid value";
							//}
						}
					}
					else
					{
						fieldErrors[""] = validationResult.ErrorMessage ?? "Validation error";
					}
				}

				_logger.LogError("Validation failed for {RequestType}: {@Errors}", typeof(TRequest).Name, fieldErrors);

				return Result.Failure(
					HttpResponseStatusCodes.BadRequest,
					Error.BadRequest("Invalid information. Please check the highlighted fields.", fieldErrors)
				);
			}


			return Result.Success();
		}

		private string ToCamelCase(string input)
		{
			if (string.IsNullOrWhiteSpace(input) || input.Length < 2)
				return input.ToLowerInvariant();

			return char.ToLowerInvariant(input[0]) + input.Substring(1);
		}
	}
}
