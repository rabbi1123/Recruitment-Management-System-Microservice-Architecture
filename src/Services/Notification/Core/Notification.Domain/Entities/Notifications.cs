using Common.Platform.Domain.Abstractions;

namespace Notification.Domain.Entities;

public sealed class Notifications : IEntity
{
	public Notifications()
	{
	}

	public string GetPrimaryKeyName() => nameof(Id);

	public long GetPrimaryKeyValue() => Id;

	public long Id { get; set; }

	public Guid EventId { get; set; }

	public long? UserId { get; set; }

	public string Channel { get; set; } = string.Empty;

	public string Recipient { get; set; } = string.Empty;

	public string? TemplateName { get; set; }

	public int? TemplateVersion { get; set; }

	public string? Title { get; set; }

	public string? Subject { get; set; }

	public string? Body { get; set; }

	public string Status { get; set; } = string.Empty;

	public int RetryCount { get; set; }

	public string? Provider { get; set; }

	public string? ProviderMessageId { get; set; }

	public string? ErrorCode { get; set; }

	public string? ErrorMessage { get; set; }

	public DateTime? ScheduledOn { get; set; }

	public DateTime? SentOn { get; set; }

	public DateTime? DeliveredOn { get; set; }

	public DateTime CreatedOn { get; set; }

	public string? CreatedBy { get; set; }

	public DateTime? UpdatedOn { get; set; }

	public string? UpdatedBy { get; set; }

	public static Notifications Sent(
		Guid eventId,
		string channel,
		string recipient,
		string templateName,
		string? title,
		string? subject,
		string body,
		string provider,
		string? providerMessageId)
	{
		return new Notifications
		{
			EventId = eventId,
			UserId = null,
			Channel = channel,
			Recipient = recipient,
			TemplateName = templateName,
			TemplateVersion = 1,
			Title = title,
			Subject = subject,
			Body = body,
			Status = "SENT",
			RetryCount = 0,
			Provider = provider,
			ProviderMessageId = providerMessageId,
			SentOn = DateTime.UtcNow,
			CreatedOn = DateTime.UtcNow,
			CreatedBy = "NotificationService"
		};
	}
}
