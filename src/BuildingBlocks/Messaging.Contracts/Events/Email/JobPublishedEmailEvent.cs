namespace Messaging.Contracts.Events.Email;

public sealed record JobPublishedEmailEvent(long JobId, long OrganizationId, string Title, DateTime PublishedOnUtc);
