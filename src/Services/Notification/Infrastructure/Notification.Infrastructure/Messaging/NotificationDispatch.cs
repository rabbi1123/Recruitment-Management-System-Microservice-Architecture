using Common.Platform.Domain.Abstractions;
using MassTransit;
using MediatR;
using Notification.Application.Common;

namespace Notification.Infrastructure.Messaging;

internal static class NotificationDispatch
{
	public static Task SendEmailAsync(IMediator mediator, ConsumeContext context, NotificationEventData data) =>
		SendAsync(mediator, PublishedEventMapper.ToEmail(EventId(context), data), context.CancellationToken);

	public static Task SendSmsAsync(IMediator mediator, ConsumeContext context, NotificationEventData data) =>
		SendAsync(mediator, PublishedEventMapper.ToSms(EventId(context), data), context.CancellationToken);

	public static Task SendPushAsync(IMediator mediator, ConsumeContext context, NotificationEventData data) =>
		SendAsync(mediator, PublishedEventMapper.ToPush(EventId(context), data), context.CancellationToken);

	private static Guid EventId(ConsumeContext context) => context.MessageId ?? Guid.NewGuid();

	private static async Task SendAsync<TCommand>(IMediator mediator, TCommand command, CancellationToken cancellationToken)
		where TCommand : IRequest<Result<CommandResponse>>
	{
		var result = await mediator.Send(command, cancellationToken);
		if (result.IsFailure)
		{
			throw new InvalidOperationException(result.Error?.Message ?? "Failed to store the notification.");
		}
	}
}
