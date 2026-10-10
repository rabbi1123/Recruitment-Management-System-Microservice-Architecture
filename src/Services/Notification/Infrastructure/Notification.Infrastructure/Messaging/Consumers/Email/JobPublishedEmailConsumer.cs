using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Email;

namespace Notification.Infrastructure.Messaging.Consumers.Email;

public sealed class JobPublishedEmailConsumer : IConsumer<JobPublishedEmailEvent>
{
	private readonly IMediator _mediator;

	public JobPublishedEmailConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<JobPublishedEmailEvent> context) =>
		NotificationDispatch.SendEmailAsync(_mediator, context, PublishedEventMapper.From(context.Message));
}
