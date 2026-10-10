using Common.Platform.Domain.Abstractions;
using MediatR;
using Notification.Application.Common;
using Notification.Application.Interfaces;

namespace Notification.Application.Features.Notifications.Commands;

public sealed class SendPushNotificationCommandHandler
	: IRequestHandler<SendPushNotificationCommand, Result<CommandResponse>>
{
	private readonly INotificationRepository _repository;
	private readonly IEmailTemplateService _templates;
	private readonly IPushService _pushService;

	public SendPushNotificationCommandHandler(
		INotificationRepository repository,
		IEmailTemplateService templates,
		IPushService pushService)
	{
		_repository = repository;
		_templates = templates;
		_pushService = pushService;
	}

	public Task<Result<CommandResponse>> Handle(SendPushNotificationCommand request, CancellationToken cancellationToken)
	{
		return SendNotificationHandler.HandleAsync(
			_repository,
			_templates,
			request.EventId,
			NotificationChannels.Push,
			request.Recipient,
			request.TemplateName,
			request.Tokens,
			content => (content.Title, null, content.Body),
			(content, ct) => _pushService.SendAsync(request.Recipient, content.Title, content.Body, ct),
			cancellationToken);
	}
}
