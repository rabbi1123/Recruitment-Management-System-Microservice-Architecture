using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Email;

namespace Notification.Infrastructure.Messaging.Consumers.Email;

public sealed class OfferSentEmailConsumer : IConsumer<OfferSentEmailEvent>
{
	private readonly IMediator _mediator;

	public OfferSentEmailConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<OfferSentEmailEvent> context) =>
		NotificationDispatch.SendEmailAsync(_mediator, context, PublishedEventMapper.From(context.Message));
}
