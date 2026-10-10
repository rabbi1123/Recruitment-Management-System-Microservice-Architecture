namespace Notification.Application.Interfaces;

public interface IEmailService
{
	Task<DeliveryResult> SendAsync(string recipient, string subject, string body, CancellationToken cancellationToken);
}
