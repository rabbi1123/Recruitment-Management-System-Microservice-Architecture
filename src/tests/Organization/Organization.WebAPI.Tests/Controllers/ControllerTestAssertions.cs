using Common.Platform.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Organization.WebAPI.Tests.Controllers;

internal static class ControllerTestAssertions
{
	public static void AssertOkResult<T>(IActionResult result)
	{
		var okResult = Assert.IsType<OkObjectResult>(result);
		var value = Assert.IsType<Result<T>>(okResult.Value);
		Assert.True(value.IsSuccess);
	}

	public static void AssertStatusCodeResult<T>(IActionResult result, HttpResponseStatusCodes expectedStatus)
	{
		var objectResult = Assert.IsType<ObjectResult>(result);
		Assert.Equal((int)expectedStatus, objectResult.StatusCode);
		var value = Assert.IsType<Result<T>>(objectResult.Value);
		Assert.True(value.IsFailure);
	}
}
