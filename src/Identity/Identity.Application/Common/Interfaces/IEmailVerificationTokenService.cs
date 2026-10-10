namespace Identity.Application.Common.Interfaces;

public interface IEmailVerificationTokenService
{
    Task<string> IssueAsync(Guid userId, CancellationToken cancellationToken = default);

    Task VerifyAsync(string rawToken, CancellationToken cancellationToken = default);
}
