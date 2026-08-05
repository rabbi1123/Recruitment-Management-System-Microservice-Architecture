using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Models;
using MediatR;

namespace Identity.Application.Features.Roles.AssignRole;

public sealed record AssignRoleCommand(Guid UserId, string Role) : IRequest<ApiResponse>;

public sealed class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, ApiResponse>
{
    private readonly IIdentityService _identityService;

    public AssignRoleCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ApiResponse> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        await _identityService.AssignRoleAsync(request.UserId, request.Role, cancellationToken);
        return ApiResponse.Ok("Role assigned successfully.");
    }
}
