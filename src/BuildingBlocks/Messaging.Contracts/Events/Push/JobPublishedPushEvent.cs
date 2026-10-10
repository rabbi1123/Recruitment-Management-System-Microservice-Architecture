namespace Messaging.Contracts.Events.Push;

public sealed record JobPublishedPushEvent(long JobId, long OrganizationId, string Title, DateTime PublishedOnUtc);
