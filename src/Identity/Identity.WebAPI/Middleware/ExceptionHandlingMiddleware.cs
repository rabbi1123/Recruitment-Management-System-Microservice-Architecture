using System.Text.Json;
using Identity.Application.Common.Exceptions;
using Identity.Application.Common.Models;

namespace Identity.WebAPI.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int statusCode;
        ApiResponse response;

        if (exception is AppException appEx)
        {
            statusCode = appEx.StatusCode;
            response = ApiResponse.Fail(appEx.Message, appEx.Errors);
        }
        else
        {
            statusCode = Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError;
            response = ApiResponse.Fail("An unexpected error occurred.");
        }

        if (statusCode >= 500)
        {
            _logger.LogError(exception, "Unhandled exception");
        }
        else
        {
            _logger.LogWarning(exception, "Handled application exception: {Message}", exception.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
