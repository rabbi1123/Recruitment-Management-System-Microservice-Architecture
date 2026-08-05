using Identity.Domain.Entities;
using System.Security.Claims;

namespace Identity.Application.Common.Interfaces;

public interface IJwtService
{
    Task<(string Token, DateTime ExpiresAt)> GenerateAccessTokenAsync(
        ApplicationUser user,
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
