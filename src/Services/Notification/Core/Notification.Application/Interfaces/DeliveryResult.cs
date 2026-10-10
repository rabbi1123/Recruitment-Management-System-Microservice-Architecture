namespace Notification.Application.Interfaces;

public sealed record DeliveryResult(string Provider, string? ProviderMessageId);
