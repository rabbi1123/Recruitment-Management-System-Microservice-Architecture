using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Models;
using Identity.Application.Features.Auth.Common;
using MediatR;

namespace Identity.Application.Features.Auth.Register;

public sealed record RegisterCommand(string Email, string Password)
    : IRequest<ApiResponse<AuthTokensResponse>>;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<AuthTokensResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly IIpAddressAccessor _ipAddressAccessor;

    public RegisterCommandHandler(
        IIdentityService identityService,
        ITokenService tokenService,
        IIpAddressAccessor ipAddressAccessor)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _ipAddressAccessor = ipAddressAccessor;
    }

    public async Task<ApiResponse<AuthTokensResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.RegisterAsync(request.Email, request.Password, cancellationToken);
        var tokens = await _tokenService.IssueTokensAsync(user, _ipAddressAccessor.GetIpAddress(), cancellationToken);
        return ApiResponse<AuthTokensResponse>.Ok(tokens, "Registration successful");
    }
}
