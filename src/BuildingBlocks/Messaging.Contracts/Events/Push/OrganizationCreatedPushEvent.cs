namespace Messaging.Contracts.Events.Push;

public sealed record OrganizationCreatedPushEvent(long OrganizationId, string Name, DateTime OccurredOnUtc);
