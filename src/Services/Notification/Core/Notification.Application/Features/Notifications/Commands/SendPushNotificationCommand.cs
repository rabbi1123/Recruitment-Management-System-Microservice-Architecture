using Common.Platform.Domain.Abstractions;
using MediatR;
using Notification.Application.Common;

namespace Notification.Application.Features.Notifications.Commands;

public sealed record SendPushNotificationCommand : IRequest<Result<CommandResponse>>
{
	public Guid EventId { get; init; }

	public string TemplateName { get; init; } = string.Empty;

	public string Recipient { get; init; } = string.Empty;

	public Dictionary<string, string> Tokens { get; init; } = new();
}
