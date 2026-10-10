using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Sms;

namespace Notification.Infrastructure.Messaging.Consumers.Sms;

public sealed class OrganizationCreatedSmsConsumer : IConsumer<OrganizationCreatedSmsEvent>
{
	private readonly IMediator _mediator;

	public OrganizationCreatedSmsConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<OrganizationCreatedSmsEvent> context) =>
		NotificationDispatch.SendSmsAsync(_mediator, context, PublishedEventMapper.From(context.Message));
}
