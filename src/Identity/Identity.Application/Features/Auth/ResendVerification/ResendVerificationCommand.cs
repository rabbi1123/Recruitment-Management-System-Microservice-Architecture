using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Models;
using MediatR;

namespace Identity.Application.Features.Auth.ResendVerification;

public sealed record ResendVerificationCommand(string Email) : IRequest<ApiResponse>;

public sealed class ResendVerificationCommandHandler : IRequestHandler<ResendVerificationCommand, ApiResponse>
{
    private readonly IIdentityService _identityService;

    public ResendVerificationCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ApiResponse> Handle(ResendVerificationCommand request, CancellationToken cancellationToken)
    {
        await _identityService.ResendVerificationAsync(request.Email, cancellationToken);
        return ApiResponse.Ok("If an account with that email exists and is not yet verified, a verification link has been sent.");
    }
}
