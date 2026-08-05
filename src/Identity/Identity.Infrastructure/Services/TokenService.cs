using Identity.Application.Common.Interfaces;
using Identity.Application.Features.Auth.Common;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services;

public sealed class TokenService : ITokenService
{
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public TokenService(
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        UserManager<ApplicationUser> userManager)
    {
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _userManager = userManager;
    }

    public async Task<AuthTokensResponse> IssueTokensAsync(
        ApplicationUser user,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var (accessToken, accessExpires) = await _jwtService.GenerateAccessTokenAsync(user, roles, cancellationToken);
        var (rawRefresh, refreshEntity) = await _refreshTokenService.CreateAsync(user.Id, ipAddress, cancellationToken);

        return new AuthTokensResponse(accessToken, accessExpires, rawRefresh, refreshEntity.Expires);
    }
}
