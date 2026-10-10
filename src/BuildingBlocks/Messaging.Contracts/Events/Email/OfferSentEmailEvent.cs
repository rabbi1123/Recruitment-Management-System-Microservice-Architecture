namespace Messaging.Contracts.Events.Email;

public sealed record OfferSentEmailEvent(
    long OfferId,
    long ApplicationId,
    long CandidateId,
    long JobId,
    long OrganizationId,
    DateTime SentOnUtc);
