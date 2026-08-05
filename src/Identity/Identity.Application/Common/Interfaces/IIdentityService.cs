using Identity.Application.Features.Auth.Common;
using Identity.Domain.Entities;

namespace Identity.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<ApplicationUser> RegisterAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<ApplicationUser> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    Task ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);

    Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);

    Task AssignRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    Task RemoveRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<ApplicationUser?> FindByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
