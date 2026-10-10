namespace Messaging.Contracts.Events.Email;

public sealed record UserVerificationEmailEvent(Guid UserId, string Email, string VerificationToken);
