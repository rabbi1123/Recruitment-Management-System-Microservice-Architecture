using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Email;

namespace Notification.Infrastructure.Messaging.Consumers.Email;

public sealed class EmployeeRegisteredEmailConsumer : IConsumer<EmployeeRegisteredEmailEvent>
{
	private readonly IMediator _mediator;

	public EmployeeRegisteredEmailConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<EmployeeRegisteredEmailEvent> context) =>
		NotificationDispatch.SendEmailAsync(_mediator, context, PublishedEventMapper.From(context.Message));
}
