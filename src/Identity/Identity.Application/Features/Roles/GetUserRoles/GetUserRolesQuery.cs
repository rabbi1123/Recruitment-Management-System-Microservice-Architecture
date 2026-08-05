using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Models;
using MediatR;

namespace Identity.Application.Features.Roles.GetUserRoles;

public sealed record GetUserRolesQuery(Guid UserId) : IRequest<ApiResponse<IReadOnlyList<string>>>;

public sealed class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, ApiResponse<IReadOnlyList<string>>>
{
    private readonly IIdentityService _identityService;

    public GetUserRolesQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ApiResponse<IReadOnlyList<string>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _identityService.GetUserRolesAsync(request.UserId, cancellationToken);
        return ApiResponse<IReadOnlyList<string>>.Ok(roles, "User roles retrieved successfully.");
    }
}
