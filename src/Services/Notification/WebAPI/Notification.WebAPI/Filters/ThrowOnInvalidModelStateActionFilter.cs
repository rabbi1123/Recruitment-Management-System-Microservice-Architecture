using Common.Platform.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Notification.WebAPI.Filters;

public sealed class ThrowOnInvalidModelStateActionFilter : IActionFilter
{
	public void OnActionExecuting(ActionExecutingContext context)
	{
		if (context.ModelState.IsValid)
			return;

		var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (var entry in context.ModelState)
		{
			var error = entry.Value?.Errors.FirstOrDefault();
			if (error is null)
				continue;

			fields[entry.Key] = string.IsNullOrWhiteSpace(error.ErrorMessage)
				? "Invalid value."
				: error.ErrorMessage;
		}

		throw new ValidationException(
			Result.Failure(
				HttpResponseStatusCodes.BadRequest,
				Error.BadRequest("Invalid information. Please check the highlighted fields.", fields)).Error!);
	}

	public void OnActionExecuted(ActionExecutedContext context)
	{
	}
}
