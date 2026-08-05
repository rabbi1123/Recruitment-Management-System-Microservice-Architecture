namespace Identity.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetAsync(string email, string resetToken, CancellationToken cancellationToken = default);
}
