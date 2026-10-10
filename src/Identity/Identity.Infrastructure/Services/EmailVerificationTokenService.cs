using System.Security.Cryptography;
using System.Text;
using Identity.Application.Common.Exceptions;
using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Options;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Services;

public sealed class EmailVerificationTokenService : IEmailVerificationTokenService
{
    private const string InvalidTokenMessage = "This verification link is invalid or has expired.";

    private readonly IdentityDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IdentitySettings _settings;

    public EmailVerificationTokenService(
        IdentityDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IOptions<IdentitySettings> settings)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _settings = settings.Value;
    }

    public async Task<string> IssueAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var unused = await _dbContext.EmailVerificationTokens
            .Where(token => token.UserId == userId && token.UsedOn == null)
            .ToListAsync(cancellationToken);

        foreach (var token in unused)
        {
            token.UsedOn = now;
        }

        var rawToken = CreateRawToken();
        _dbContext.EmailVerificationTokens.Add(new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = HashToken(rawToken),
            ExpiresOn = now.AddMinutes(_settings.EmailVerificationTokenMinutes),
            CreatedOn = now
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        return rawToken;
    }

    public async Task VerifyAsync(string rawToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
        {
            throw new BadRequestException(InvalidTokenMessage);
        }

        var tokenHash = HashToken(rawToken.Trim());
        var stored = await _dbContext.EmailVerificationTokens
            .FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

        if (stored is null || stored.UsedOn is not null || stored.ExpiresOn <= DateTime.UtcNow)
        {
            throw new BadRequestException(InvalidTokenMessage);
        }

        var user = await _userManager.FindByIdAsync(stored.UserId.ToString());
        if (user is null)
        {
            throw new BadRequestException(InvalidTokenMessage);
        }

        var now = DateTime.UtcNow;
        user.EmailConfirmed = true;
        user.UpdatedOn = now;
        stored.UsedOn = now;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new BadRequestException(
                "Email verification failed.",
                result.Errors.Select(error => error.Description));
        }
    }

    private static string CreateRawToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string HashToken(string rawToken)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
