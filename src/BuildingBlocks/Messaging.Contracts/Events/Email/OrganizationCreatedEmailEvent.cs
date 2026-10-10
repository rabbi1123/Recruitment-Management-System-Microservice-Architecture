namespace Messaging.Contracts.Events.Email;

public sealed record OrganizationCreatedEmailEvent(long OrganizationId, string Name, DateTime OccurredOnUtc);
