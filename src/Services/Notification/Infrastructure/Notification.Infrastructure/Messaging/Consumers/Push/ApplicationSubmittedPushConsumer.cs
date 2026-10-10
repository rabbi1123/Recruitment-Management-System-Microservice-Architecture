using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Push;

namespace Notification.Infrastructure.Messaging.Consumers.Push;

public sealed class ApplicationSubmittedPushConsumer : IConsumer<ApplicationSubmittedPushEvent>
{
	private readonly IMediator _mediator;

	public ApplicationSubmittedPushConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<ApplicationSubmittedPushEvent> context) =>
		NotificationDispatch.SendPushAsync(_mediator, context, PublishedEventMapper.From(context.Message));
}
