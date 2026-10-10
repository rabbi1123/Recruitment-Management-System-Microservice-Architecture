namespace Messaging.Contracts.Events.Sms;

public sealed record JobPublishedSmsEvent(long JobId, long OrganizationId, string Title, DateTime PublishedOnUtc);
