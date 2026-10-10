using Microsoft.Extensions.Logging;
using Notification.Application.Interfaces;

namespace Notification.Infrastructure.Push;

public sealed class PushService : IPushService
{
	private readonly ILogger<PushService> _logger;

	public PushService(ILogger<PushService> logger)
	{
		_logger = logger;
	}

	public Task<DeliveryResult> SendAsync(string recipient, string title, string body, CancellationToken cancellationToken)
	{
		var providerMessageId = Guid.NewGuid().ToString("N");
		_logger.LogInformation(
			"Push to {Recipient}. Title: {Title}. Body: {Body}. Provider message {ProviderMessageId}",
			recipient,
			title,
			body,
			providerMessageId);
		return Task.FromResult(new DeliveryResult("Log", providerMessageId));
	}
}
