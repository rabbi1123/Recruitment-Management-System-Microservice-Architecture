namespace Identity.Application.Common.Exceptions;

public abstract class AppException : Exception
{
    public int StatusCode { get; }
    public IReadOnlyList<string> Errors { get; }

    protected AppException(string message, int statusCode, IEnumerable<string>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors?.ToList() ?? [];
    }
}

public sealed class BadRequestException : AppException
{
    public BadRequestException(string message, IEnumerable<string>? errors = null)
        : base(message, AppStatusCodes.BadRequest, errors)
    {
    }
}

public sealed class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Unauthorized")
        : base(message, AppStatusCodes.Unauthorized)
    {
    }
}

public sealed class ForbiddenException : AppException
{
    public ForbiddenException(string message = "Forbidden")
        : base(message, AppStatusCodes.Forbidden)
    {
    }
}

public sealed class NotFoundException : AppException
{
    public NotFoundException(string message = "Resource not found")
        : base(message, AppStatusCodes.NotFound)
    {
    }
}

public sealed class ConflictException : AppException
{
    public ConflictException(string message, IEnumerable<string>? errors = null)
        : base(message, AppStatusCodes.Conflict, errors)
    {
    }
}

public sealed class ValidationException : AppException
{
    public ValidationException(IEnumerable<string> errors)
        : base("Validation failed", AppStatusCodes.BadRequest, errors)
    {
    }
}

public static class AppStatusCodes
{
    public const int BadRequest = 400;
    public const int Unauthorized = 401;
    public const int Forbidden = 403;
    public const int NotFound = 404;
    public const int Conflict = 409;
    public const int TooManyRequests = 429;
    public const int InternalServerError = 500;
}
