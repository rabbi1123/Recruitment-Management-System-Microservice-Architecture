namespace Messaging.Contracts.Events.Push;

public sealed record ApplicationSubmittedPushEvent(
    long ApplicationId,
    long JobId,
    long CandidateId,
    long OrganizationId,
    DateTime OccurredOnUtc);
