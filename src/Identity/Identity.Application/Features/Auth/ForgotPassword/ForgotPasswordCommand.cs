using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Models;
using MediatR;

namespace Identity.Application.Features.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<ApiResponse>;

public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ApiResponse>
{
    private readonly IIdentityService _identityService;

    public ForgotPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ApiResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        await _identityService.ForgotPasswordAsync(request.Email, cancellationToken);
        return ApiResponse.Ok("If an account with that email exists, a password reset link has been sent.");
    }
}
