namespace Messaging.Contracts.Events.Sms;

public sealed record OfferSentSmsEvent(
    long OfferId,
    long ApplicationId,
    long CandidateId,
    long JobId,
    long OrganizationId,
    DateTime SentOnUtc);
