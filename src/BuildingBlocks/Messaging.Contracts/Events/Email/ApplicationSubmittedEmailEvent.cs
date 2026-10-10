namespace Messaging.Contracts.Events.Email;

public sealed record ApplicationSubmittedEmailEvent(
    long ApplicationId,
    long JobId,
    long CandidateId,
    long OrganizationId,
    DateTime OccurredOnUtc);
