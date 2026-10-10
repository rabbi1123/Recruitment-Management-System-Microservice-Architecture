using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Push;

namespace Notification.Infrastructure.Messaging.Consumers.Push;

public sealed class JobPublishedPushConsumer : IConsumer<JobPublishedPushEvent>
{
	private readonly IMediator _mediator;

	public JobPublishedPushConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<JobPublishedPushEvent> context) =>
		NotificationDispatch.SendPushAsync(_mediator, context, PublishedEventMapper.From(context.Message));
}
