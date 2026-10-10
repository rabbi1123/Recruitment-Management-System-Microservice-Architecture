namespace Notification.Application.Interfaces;

public interface ISmsService
{
	Task<DeliveryResult> SendAsync(string recipient, string body, CancellationToken cancellationToken);
}
