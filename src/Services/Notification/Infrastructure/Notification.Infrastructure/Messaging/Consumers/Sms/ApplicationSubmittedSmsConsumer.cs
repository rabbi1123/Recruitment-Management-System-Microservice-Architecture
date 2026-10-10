using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Sms;

namespace Notification.Infrastructure.Messaging.Consumers.Sms;

public sealed class ApplicationSubmittedSmsConsumer : IConsumer<ApplicationSubmittedSmsEvent>
{
	private readonly IMediator _mediator;

	public ApplicationSubmittedSmsConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<ApplicationSubmittedSmsEvent> context) =>
		NotificationDispatch.SendSmsAsync(_mediator, context, PublishedEventMapper.From(context.Message));
}
