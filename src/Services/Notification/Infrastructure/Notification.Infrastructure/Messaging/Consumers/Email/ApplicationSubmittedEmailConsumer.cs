using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Email;

namespace Notification.Infrastructure.Messaging.Consumers.Email;

public sealed class ApplicationSubmittedEmailConsumer : IConsumer<ApplicationSubmittedEmailEvent>
{
	private readonly IMediator _mediator;

	public ApplicationSubmittedEmailConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<ApplicationSubmittedEmailEvent> context) =>
		NotificationDispatch.SendEmailAsync(_mediator, context, PublishedEventMapper.From(context.Message));
}
