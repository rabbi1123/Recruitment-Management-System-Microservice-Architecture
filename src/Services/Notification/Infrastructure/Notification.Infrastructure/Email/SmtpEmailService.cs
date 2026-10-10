using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notification.Application.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Notification.Infrastructure.Email;

public sealed class SmtpEmailService : IEmailService
{
	private readonly SmtpSettings _settings;
	private readonly ILogger<SmtpEmailService> _logger;

	public SmtpEmailService(IOptions<SmtpSettings> settings, ILogger<SmtpEmailService> logger)
	{
		_settings = settings.Value;
		_logger = logger;
	}

	public async Task<DeliveryResult> SendAsync(string recipient, string subject, string body, CancellationToken cancellationToken)
	{
		var smtpRecipient = ToSmtpAddress(recipient);
		var smtpBody = smtpRecipient == recipient
			? body
			: $"Stored recipient: {recipient}{Environment.NewLine}{Environment.NewLine}{body}";

		using var message = new MailMessage(_settings.From, smtpRecipient, subject, smtpBody);
		using var client = CreateClient();

		await client.SendMailAsync(message, cancellationToken);

		var providerMessageId = Guid.NewGuid().ToString("N");
		_logger.LogInformation(
			"Sent email to {Recipient} via SMTP {Host}:{Port}. Provider message {ProviderMessageId}",
			recipient,
			_settings.Host,
			_settings.Port,
			providerMessageId);

		return new DeliveryResult("Smtp", providerMessageId);
	}

	private SmtpClient CreateClient()
	{
#pragma warning disable SYSLIB0014
		var client = new SmtpClient(_settings.Host, _settings.Port)
		{
			EnableSsl = _settings.EnableSsl,
			DeliveryMethod = SmtpDeliveryMethod.Network,
			UseDefaultCredentials = false
		};
#pragma warning restore SYSLIB0014

		if (!string.IsNullOrWhiteSpace(_settings.Username))
			client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);

		return client;
	}

	private static string ToSmtpAddress(string recipient)
	{
		if (MailAddress.TryCreate(recipient, out _))
			return recipient;

		var local = new string(recipient
			.Select(character => char.IsLetterOrDigit(character) ? char.ToLowerInvariant(character) : '-')
			.ToArray()).Trim('-');

		if (string.IsNullOrWhiteSpace(local))
			local = "notification";

		return $"{local}@notifications.local";
	}
}
