namespace Messaging.Contracts.Events.Push;

public sealed record OfferSentPushEvent(
    long OfferId,
    long ApplicationId,
    long CandidateId,
    long JobId,
    long OrganizationId,
    DateTime SentOnUtc);
