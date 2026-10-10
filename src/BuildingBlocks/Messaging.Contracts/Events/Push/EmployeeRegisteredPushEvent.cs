namespace Messaging.Contracts.Events.Push;

public sealed record EmployeeRegisteredPushEvent(Guid UserId, string Email);
