namespace Messaging.Contracts.Events.Sms;

public sealed record EmployeeRegisteredSmsEvent(Guid UserId, string Email);
