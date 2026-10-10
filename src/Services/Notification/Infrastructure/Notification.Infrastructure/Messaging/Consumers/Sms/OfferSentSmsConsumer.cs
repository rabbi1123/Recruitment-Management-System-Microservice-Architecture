using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Sms;

namespace Notification.Infrastructure.Messaging.Consumers.Sms;

public sealed class OfferSentSmsConsumer : IConsumer<OfferSentSmsEvent>
{
	private readonly IMediator _mediator;

	public OfferSentSmsConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<OfferSentSmsEvent> context) =>
		NotificationDispatch.SendSmsAsync(_mediator, context, PublishedEventMapper.From(context.Message));
}
