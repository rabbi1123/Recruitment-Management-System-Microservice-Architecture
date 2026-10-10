namespace Messaging.Contracts.Events.Email;

public sealed record EmployeeRegisteredEmailEvent(Guid UserId, string Email);
