namespace Messaging.Contracts.Events.Sms;

public sealed record ApplicationSubmittedSmsEvent(
    long ApplicationId,
    long JobId,
    long CandidateId,
    long OrganizationId,
    DateTime OccurredOnUtc);
