using Identity.Application.Common.Interfaces;

namespace Identity.WebAPI.Services;

public sealed class IpAddressAccessor : IIpAddressAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IpAddressAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetIpAddress()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
        {
            return null;
        }

        var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwarded))
        {
            return forwarded.Split(',')[0].Trim();
        }

        return context.Connection.RemoteIpAddress?.ToString();
    }
}
