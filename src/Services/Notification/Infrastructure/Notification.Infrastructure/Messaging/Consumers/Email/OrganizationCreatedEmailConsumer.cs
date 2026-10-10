using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Email;

namespace Notification.Infrastructure.Messaging.Consumers.Email;

public sealed class OrganizationCreatedEmailConsumer : IConsumer<OrganizationCreatedEmailEvent>
{
	private readonly IMediator _mediator;

	public OrganizationCreatedEmailConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<OrganizationCreatedEmailEvent> context) =>
		NotificationDispatch.SendEmailAsync(_mediator, context, PublishedEventMapper.From(context.Message));
}
