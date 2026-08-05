using Identity.Application.Features.Auth.Common;
using Identity.Domain.Entities;

namespace Identity.Application.Common.Interfaces;

public interface ITokenService
{
    Task<AuthTokensResponse> IssueTokensAsync(
        ApplicationUser user,
        string? ipAddress,
        CancellationToken cancellationToken = default);
}
