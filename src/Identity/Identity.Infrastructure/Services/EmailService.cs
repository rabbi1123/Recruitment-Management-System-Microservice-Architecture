using Identity.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Services;

public sealed class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendPasswordResetAsync(string email, string resetToken, CancellationToken cancellationToken = default)
    {
        // Mock email provider — logs token for local/dev testing only.
        _logger.LogInformation(
            "Password reset email for {Email}. Reset token: {Token}",
            email,
            resetToken);

        return Task.CompletedTask;
    }
}
