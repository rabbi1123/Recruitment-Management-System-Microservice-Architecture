using Common.Platform.Domain.Abstractions;
using MediatR;
using Notification.Application.Common;
using Notification.Application.Interfaces;

namespace Notification.Application.Features.Notifications.Commands;

public sealed class SendEmailNotificationCommandHandler
	: IRequestHandler<SendEmailNotificationCommand, Result<CommandResponse>>
{
	private readonly INotificationRepository _repository;
	private readonly IEmailTemplateService _templates;
	private readonly IEmailService _emailService;

	public SendEmailNotificationCommandHandler(
		INotificationRepository repository,
		IEmailTemplateService templates,
		IEmailService emailService)
	{
		_repository = repository;
		_templates = templates;
		_emailService = emailService;
	}

	public Task<Result<CommandResponse>> Handle(SendEmailNotificationCommand request, CancellationToken cancellationToken)
	{
		return SendNotificationHandler.HandleAsync(
			_repository,
			_templates,
			request.EventId,
			NotificationChannels.Email,
			request.Recipient,
			request.TemplateName,
			request.Tokens,
			content => (null, content.Subject, content.Body),
			(content, ct) => _emailService.SendAsync(request.Recipient, content.Subject, content.Body, ct),
			cancellationToken);
	}
}
