using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Email;
using Microsoft.Extensions.Options;
using Notification.Infrastructure.Email;

namespace Notification.Infrastructure.Messaging.Consumers.Email;

public sealed class UserVerificationEmailConsumer : IConsumer<UserVerificationEmailEvent>
{
	private readonly IMediator _mediator;
	private readonly PublicWebSettings _settings;

	public UserVerificationEmailConsumer(IMediator mediator, IOptions<PublicWebSettings> settings)
	{
		_mediator = mediator;
		_settings = settings.Value;
	}

	public Task Consume(ConsumeContext<UserVerificationEmailEvent> context) =>
		NotificationDispatch.SendEmailAsync(
			_mediator,
			context,
			PublishedEventMapper.From(context.Message, _settings.PublicWebBaseUrl));
}
