using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Models;
using MediatR;

namespace Identity.Application.Features.Roles.RemoveRole;

public sealed record RemoveRoleCommand(Guid UserId, string Role) : IRequest<ApiResponse>;

public sealed class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, ApiResponse>
{
    private readonly IIdentityService _identityService;

    public RemoveRoleCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ApiResponse> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        await _identityService.RemoveRoleAsync(request.UserId, request.Role, cancellationToken);
        return ApiResponse.Ok("Role removed successfully.");
    }
}
