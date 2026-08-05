using FluentValidation;
using DomainRoles = Identity.Domain.Constants.Roles;

namespace Identity.Application.Features.Roles.RemoveRole;

public sealed class RemoveRoleCommandValidator : AbstractValidator<RemoveRoleCommand>
{
    public RemoveRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(role => DomainRoles.All.Contains(role))
            .WithMessage($"Role must be one of: {string.Join(", ", DomainRoles.All)}.");
    }
}
