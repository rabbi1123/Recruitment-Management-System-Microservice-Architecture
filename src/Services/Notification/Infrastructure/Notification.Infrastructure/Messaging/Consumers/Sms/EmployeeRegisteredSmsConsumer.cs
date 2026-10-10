using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Sms;

namespace Notification.Infrastructure.Messaging.Consumers.Sms;

public sealed class EmployeeRegisteredSmsConsumer : IConsumer<EmployeeRegisteredSmsEvent>
{
	private readonly IMediator _mediator;

	public EmployeeRegisteredSmsConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<EmployeeRegisteredSmsEvent> context) =>
		NotificationDispatch.SendSmsAsync(_mediator, context, PublishedEventMapper.From(context.Message));
}
