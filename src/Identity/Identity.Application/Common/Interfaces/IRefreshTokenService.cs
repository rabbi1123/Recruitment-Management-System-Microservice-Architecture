using Identity.Domain.Entities;

namespace Identity.Application.Common.Interfaces;

public interface IRefreshTokenService
{
    Task<(string RawToken, RefreshToken Entity)> CreateAsync(
        Guid userId,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<RefreshToken?> FindByRawTokenAsync(
        string rawToken,
        CancellationToken cancellationToken = default);

    Task<(string RawToken, RefreshToken Entity)> RotateAsync(
        RefreshToken existingToken,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task RevokeAsync(
        RefreshToken token,
        string? ipAddress,
        string? replacedByToken = null,
        CancellationToken cancellationToken = default);

    Task RevokeAllForUserAsync(
        Guid userId,
        string? ipAddress,
        CancellationToken cancellationToken = default);
}
