using System.Security.Cryptography;
using System.Text;
using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Options;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Services;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly IdentityDbContext _dbContext;
    private readonly JwtSettings _settings;

    public RefreshTokenService(IdentityDbContext dbContext, IOptions<JwtSettings> settings)
    {
        _dbContext = dbContext;
        _settings = settings.Value;
    }

    public async Task<(string RawToken, RefreshToken Entity)> CreateAsync(
        Guid userId,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var rawToken = GenerateSecureToken();
        var entity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = HashToken(rawToken),
            Expires = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays),
            CreatedOn = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };

        _dbContext.RefreshTokens.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return (rawToken, entity);
    }

    public async Task<RefreshToken?> FindByRawTokenAsync(
        string rawToken,
        CancellationToken cancellationToken = default)
    {
        var hash = HashToken(rawToken);
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == hash, cancellationToken);
    }

    public async Task<(string RawToken, RefreshToken Entity)> RotateAsync(
        RefreshToken existingToken,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var (rawToken, newEntity) = await CreateAsync(existingToken.UserId, ipAddress, cancellationToken);
        await RevokeAsync(existingToken, ipAddress, HashToken(rawToken), cancellationToken);
        return (rawToken, newEntity);
    }

    public async Task RevokeAsync(
        RefreshToken token,
        string? ipAddress,
        string? replacedByToken = null,
        CancellationToken cancellationToken = default)
    {
        if (token.IsRevoked)
        {
            return;
        }

        token.RevokedOn = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReplacedByToken = replacedByToken;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllForUserAsync(
        Guid userId,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var tokens = await _dbContext.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedOn == null && t.Expires > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.RevokedOn = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    internal static string HashToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes);
    }
}
