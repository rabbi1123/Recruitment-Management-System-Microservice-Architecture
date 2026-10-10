namespace Messaging.Contracts.Events.Sms;

public sealed record OrganizationCreatedSmsEvent(long OrganizationId, string Name, DateTime OccurredOnUtc);
