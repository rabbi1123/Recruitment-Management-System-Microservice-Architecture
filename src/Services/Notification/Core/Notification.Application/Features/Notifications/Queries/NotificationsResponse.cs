namespace Notification.Application.Features.Notifications.Queries;

public sealed class NotificationsResponse
{
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
}
