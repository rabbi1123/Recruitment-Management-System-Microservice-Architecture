using Identity.Application.Common.Exceptions;
using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Models;
using MediatR;

namespace Identity.Application.Features.Auth.RevokeToken;

public sealed record RevokeTokenCommand(string? RefreshToken)
    : IRequest<ApiResponse>;

public sealed class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, ApiResponse>
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IIpAddressAccessor _ipAddressAccessor;

    public RevokeTokenCommandHandler(
        IRefreshTokenService refreshTokenService,
        IIpAddressAccessor ipAddressAccessor)
    {
        _refreshTokenService = refreshTokenService;
        _ipAddressAccessor = ipAddressAccessor;
    }

    public async Task<ApiResponse> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new BadRequestException("Refresh token is required.");
        }

        var existing = await _refreshTokenService.FindByRawTokenAsync(request.RefreshToken, cancellationToken);
        if (existing is null || !existing.IsActive)
        {
            // Avoid token enumeration
            return ApiResponse.Ok("Token revoked successfully");
        }

        await _refreshTokenService.RevokeAsync(existing, _ipAddressAccessor.GetIpAddress(), cancellationToken: cancellationToken);
        return ApiResponse.Ok("Token revoked successfully");
    }
}
