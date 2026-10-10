using Identity.Application.Common.Exceptions;
using Identity.Application.Common.Interfaces;
using Identity.Domain.Constants;
using Identity.Domain.Entities;
using Messaging.Contracts.Events;
using Messaging.Contracts.Events.Email;
using Messaging.Contracts.Events.Push;
using Messaging.Contracts.Events.Sms;
using Messaging.RabbitMq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IIntegrationEventPublisher _eventPublisher;
    private readonly IEmailVerificationTokenService _emailVerificationTokens;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IIntegrationEventPublisher eventPublisher,
        IEmailVerificationTokenService emailVerificationTokens)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _eventPublisher = eventPublisher;
        _emailVerificationTokens = emailVerificationTokens;
    }

    public async Task<ApplicationUser> RegisterAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim();
        var existing = await _userManager.FindByEmailAsync(normalizedEmail);
        if (existing is not null)
        {
            throw new ConflictException("Unable to complete registration.");
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            UserName = normalizedEmail,
            EmailConfirmed = false,
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
            LockoutEnabled = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new BadRequestException(
                "Registration failed.",
                result.Errors.Select(e => e.Description));
        }

        if (!await _roleManager.RoleExistsAsync(Roles.Employee))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Employee));
        }

        await _userManager.AddToRoleAsync(user, Roles.Employee);

        var verificationToken = await _emailVerificationTokens.IssueAsync(user.Id, cancellationToken);

        await _eventPublisher.PublishAsync(
            new EmployeeRegisteredEvent(user.Id, user.Email!),
            cancellationToken);
        await _eventPublisher.PublishAsync(
            new EmployeeRegisteredEmailEvent(user.Id, user.Email!),
            cancellationToken);
        await _eventPublisher.PublishAsync(
            new EmployeeRegisteredSmsEvent(user.Id, user.Email!),
            cancellationToken);
        await _eventPublisher.PublishAsync(
            new EmployeeRegisteredPushEvent(user.Id, user.Email!),
            cancellationToken);
        await _eventPublisher.PublishAsync(
            new UserVerificationEmailEvent(user.Id, user.Email!, verificationToken),
            cancellationToken);

        return user;
    }

    public async Task<ApplicationUser> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email.Trim());
        if (user is null)
        {
            throw new UnauthorizedException("Invalid credentials");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException("Invalid credentials");
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            throw new UnauthorizedException("Account is locked. Please try again later.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                throw new UnauthorizedException("Account is locked. Please try again later.");
            }

            throw new UnauthorizedException("Invalid credentials");
        }

        if (!user.EmailConfirmed)
        {
            throw new UnauthorizedException("Please verify your email address before logging in.");
        }

        user.LastLogin = DateTime.UtcNow;
        user.UpdatedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return user;
    }

    public Task VerifyEmailAsync(string token, CancellationToken cancellationToken = default)
    {
        return _emailVerificationTokens.VerifyAsync(token, cancellationToken);
    }

    public async Task ResendVerificationAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email.Trim());
        if (user is null || !user.IsActive || user.EmailConfirmed)
        {
            return;
        }

        var verificationToken = await _emailVerificationTokens.IssueAsync(user.Id, cancellationToken);
        await _eventPublisher.PublishAsync(
            new UserVerificationEmailEvent(user.Id, user.Email!, verificationToken),
            cancellationToken);
    }

    public async Task ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email.Trim());
        if (user is null || !user.IsActive)
        {
            // Anti-enumeration: do nothing visible to caller
            return;
        }
    }

    public async Task ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email.Trim());
        if (user is null)
        {
            throw new BadRequestException("Invalid password reset request.");
        }

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            throw new BadRequestException(
                "Password reset failed.",
                result.Errors.Select(e => e.Description));
        }

        user.UpdatedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        // Invalidate existing sessions by updating security stamp
        await _userManager.UpdateSecurityStampAsync(user);
    }

    public async Task AssignRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await FindRequiredUserAsync(userId);
        EnsureKnownRole(role);

        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        if (await _userManager.IsInRoleAsync(user, role))
        {
            throw new ConflictException($"User already has role '{role}'.");
        }

        var result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
        {
            throw new BadRequestException("Failed to assign role.", result.Errors.Select(e => e.Description));
        }
    }

    public async Task RemoveRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await FindRequiredUserAsync(userId);
        EnsureKnownRole(role);

        if (!await _userManager.IsInRoleAsync(user, role))
        {
            throw new NotFoundException($"User does not have role '{role}'.");
        }

        var result = await _userManager.RemoveFromRoleAsync(user, role);
        if (!result.Succeeded)
        {
            throw new BadRequestException("Failed to remove role.", result.Errors.Select(e => e.Description));
        }
    }

    public async Task<IReadOnlyList<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await FindRequiredUserAsync(userId);
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    public async Task<ApplicationUser?> FindByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    private async Task<ApplicationUser> FindRequiredUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user ?? throw new NotFoundException("User not found.");
    }

    private static void EnsureKnownRole(string role)
    {
        if (!Roles.All.Contains(role))
        {
            throw new BadRequestException($"Role '{role}' is not supported.");
        }
    }
}
