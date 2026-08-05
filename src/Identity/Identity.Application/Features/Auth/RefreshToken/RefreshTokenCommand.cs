using Identity.Application.Common.Exceptions;
using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Models;
using Identity.Application.Features.Auth.Common;
using MediatR;

namespace Identity.Application.Features.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken)
    : IRequest<ApiResponse<AuthTokensResponse>>;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthTokensResponse>>
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IIdentityService _identityService;
    private readonly IJwtService _jwtService;
    private readonly IIpAddressAccessor _ipAddressAccessor;

    public RefreshTokenCommandHandler(
        IRefreshTokenService refreshTokenService,
        IIdentityService identityService,
        IJwtService jwtService,
        IIpAddressAccessor ipAddressAccessor)
    {
        _refreshTokenService = refreshTokenService;
        _identityService = identityService;
        _jwtService = jwtService;
        _ipAddressAccessor = ipAddressAccessor;
    }

    public async Task<ApiResponse<AuthTokensResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var ip = _ipAddressAccessor.GetIpAddress();
        var existing = await _refreshTokenService.FindByRawTokenAsync(request.RefreshToken, cancellationToken);

        if (existing is null)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        if (existing.IsRevoked)
        {
            await _refreshTokenService.RevokeAllForUserAsync(existing.UserId, ip, cancellationToken);
            throw new UnauthorizedException("Refresh token reuse detected. All sessions have been revoked.");
        }

        if (existing.IsExpired)
        {
            throw new UnauthorizedException("Refresh token has expired.");
        }

        var user = await _identityService.FindByIdAsync(existing.UserId, cancellationToken)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!user.IsActive)
        {
            throw new UnauthorizedException("User account is inactive.");
        }

        var (rawToken, rotated) = await _refreshTokenService.RotateAsync(existing, ip, cancellationToken);
        var roles = await _identityService.GetUserRolesAsync(user.Id, cancellationToken);
        var (accessToken, accessExpires) = await _jwtService.GenerateAccessTokenAsync(user, roles, cancellationToken);

        var response = new AuthTokensResponse(accessToken, accessExpires, rawToken, rotated.Expires);
        return ApiResponse<AuthTokensResponse>.Ok(response, "Token refreshed successfully");
    }
}
