using Common.Platform.Domain.Abstractions;
using MediatR;
using Notification.Application.Abstractions.Mapping;
using Notification.Application.Common.CRUD.Queries;
using Notification.Application.Interfaces;
using NotificationRow = Notification.Domain.Entities.Notifications;

namespace Notification.Application.Features.Notifications.Queries;

public sealed class GetByIdNotificationsQueryHandler
	: IRequestHandler<GetByIdQuery<NotificationsResponse>, Result<NotificationsResponse>>
{
	private readonly INotificationRepository _repository;
	private readonly IResponseEntityMapper<NotificationRow, NotificationsResponse> _mapper;

	public GetByIdNotificationsQueryHandler(
		INotificationRepository repository,
		IResponseEntityMapper<NotificationRow, NotificationsResponse> mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<NotificationsResponse>> Handle(
		GetByIdQuery<NotificationsResponse> request,
		CancellationToken cancellationToken)
	{
		var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

		if (entity is null)
		{
			return Result.Failure<NotificationsResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Notifications"));
		}

		return _mapper.MapToResponse(entity);
	}
}
