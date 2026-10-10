using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Push;

namespace Notification.Infrastructure.Messaging.Consumers.Push;

public sealed class OfferSentPushConsumer : IConsumer<OfferSentPushEvent>
{
	private readonly IMediator _mediator;

	public OfferSentPushConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<OfferSentPushEvent> context) =>
		NotificationDispatch.SendPushAsync(_mediator, context, PublishedEventMapper.From(context.Message));
}
