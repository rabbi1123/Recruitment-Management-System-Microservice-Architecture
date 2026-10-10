using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Models;
using MediatR;

namespace Identity.Application.Features.Auth.VerifyEmail;

public sealed record VerifyEmailCommand(string Token) : IRequest<ApiResponse>;

public sealed class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, ApiResponse>
{
    private readonly IIdentityService _identityService;

    public VerifyEmailCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ApiResponse> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        await _identityService.VerifyEmailAsync(request.Token, cancellationToken);
        return ApiResponse.Ok("Email address verified.");
    }
}
