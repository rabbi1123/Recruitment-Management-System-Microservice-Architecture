using Notification.Application.Abstractions.Mapping;
using Notification.Application.Features.Notifications.Queries;
using NotificationRow = Notification.Domain.Entities.Notifications;

namespace Notification.Application.Features.Notifications;

public sealed class NotificationsMapper : IResponseEntityMapper<NotificationRow, NotificationsResponse>
{
	public NotificationsResponse MapToResponse(NotificationRow entity)
	{
		return new NotificationsResponse
		{
			Id = entity.Id,
			EventId = entity.EventId,
			UserId = entity.UserId,
			Channel = entity.Channel,
			Recipient = entity.Recipient,
			TemplateName = entity.TemplateName,
			TemplateVersion = entity.TemplateVersion,
			Title = entity.Title,
			Subject = entity.Subject,
			Body = entity.Body,
			Status = entity.Status,
			RetryCount = entity.RetryCount,
			Provider = entity.Provider,
			ProviderMessageId = entity.ProviderMessageId,
			ErrorCode = entity.ErrorCode,
			ErrorMessage = entity.ErrorMessage,
			ScheduledOn = entity.ScheduledOn,
			SentOn = entity.SentOn,
			DeliveredOn = entity.DeliveredOn,
			CreatedOn = entity.CreatedOn,
			CreatedBy = entity.CreatedBy
		};
	}
}
