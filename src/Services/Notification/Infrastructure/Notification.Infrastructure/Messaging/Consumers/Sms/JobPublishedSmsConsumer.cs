using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Sms;

namespace Notification.Infrastructure.Messaging.Consumers.Sms;

public sealed class JobPublishedSmsConsumer : IConsumer<JobPublishedSmsEvent>
{
	private readonly IMediator _mediator;

	public JobPublishedSmsConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<JobPublishedSmsEvent> context) =>
		NotificationDispatch.SendSmsAsync(_mediator, context, PublishedEventMapper.From(context.Message));
}
