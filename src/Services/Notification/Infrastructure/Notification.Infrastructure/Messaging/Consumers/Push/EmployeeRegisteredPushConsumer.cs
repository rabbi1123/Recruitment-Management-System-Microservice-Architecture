using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Push;

namespace Notification.Infrastructure.Messaging.Consumers.Push;

public sealed class EmployeeRegisteredPushConsumer : IConsumer<EmployeeRegisteredPushEvent>
{
	private readonly IMediator _mediator;

	public EmployeeRegisteredPushConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<EmployeeRegisteredPushEvent> context) =>
		NotificationDispatch.SendPushAsync(_mediator, context, PublishedEventMapper.From(context.Message));
}
