using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Push;

namespace Notification.Infrastructure.Messaging.Consumers.Push;

public sealed class OrganizationCreatedPushConsumer : IConsumer<OrganizationCreatedPushEvent>
{
	private readonly IMediator _mediator;

	public OrganizationCreatedPushConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<OrganizationCreatedPushEvent> context) =>
		NotificationDispatch.SendPushAsync(_mediator, context, PublishedEventMapper.From(context.Message));
}
