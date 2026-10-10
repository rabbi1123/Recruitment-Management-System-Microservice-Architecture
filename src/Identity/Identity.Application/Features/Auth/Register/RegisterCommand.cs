using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Models;
using MediatR;

namespace Identity.Application.Features.Auth.Register;

public sealed record RegisterCommand(string Email, string Password) : IRequest<ApiResponse>;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ApiResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        await _identityService.RegisterAsync(request.Email, request.Password, cancellationToken);
        return ApiResponse.Ok("Registration successful. Please verify your email address.");
    }
}
