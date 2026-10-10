namespace Notification.Application.Interfaces;

public interface IPushService
{
	Task<DeliveryResult> SendAsync(string recipient, string title, string body, CancellationToken cancellationToken);
}
