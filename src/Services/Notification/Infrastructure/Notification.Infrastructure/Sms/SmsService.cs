using Microsoft.Extensions.Logging;
using Notification.Application.Interfaces;

namespace Notification.Infrastructure.Sms;

public sealed class SmsService : ISmsService
{
	private readonly ILogger<SmsService> _logger;

	public SmsService(ILogger<SmsService> logger)
	{
		_logger = logger;
	}

	public Task<DeliveryResult> SendAsync(string recipient, string body, CancellationToken cancellationToken)
	{
		var providerMessageId = Guid.NewGuid().ToString("N");
		_logger.LogInformation("SMS to {Recipient}: {Body}. Provider message {ProviderMessageId}", recipient, body, providerMessageId);
		return Task.FromResult(new DeliveryResult("Log", providerMessageId));
	}
}
