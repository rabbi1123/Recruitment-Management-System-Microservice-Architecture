namespace Messaging.Contracts.Events;

public sealed record EmployeeRegisteredEvent(Guid UserId, string Email);
