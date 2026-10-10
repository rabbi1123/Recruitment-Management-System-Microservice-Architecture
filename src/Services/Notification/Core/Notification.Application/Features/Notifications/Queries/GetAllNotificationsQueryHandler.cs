using Common.Platform.Domain.Abstractions;
using MediatR;
using Notification.Application.Abstractions.Mapping;
using Notification.Application.Common.CRUD.Queries;
using Notification.Application.Interfaces;
using NotificationRow = Notification.Domain.Entities.Notifications;

namespace Notification.Application.Features.Notifications.Queries;

public sealed class GetAllNotificationsQueryHandler
	: IRequestHandler<GetAllQuery<NotificationsResponse>, Result<List<NotificationsResponse>>>
{
	private readonly INotificationRepository _repository;
	private readonly IResponseEntityMapper<NotificationRow, NotificationsResponse> _mapper;

	public GetAllNotificationsQueryHandler(
		INotificationRepository repository,
		IResponseEntityMapper<NotificationRow, NotificationsResponse> mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public async Task<Result<List<NotificationsResponse>>> Handle(
		GetAllQuery<NotificationsResponse> request,
		CancellationToken cancellationToken)
	{
		var entities = await _repository.GetAllAsync(
			request.SortBy?.PropertyName,
			request.SortBy?.SortBy,
			cancellationToken);

		return entities.Select(entity => _mapper.MapToResponse(entity)).ToList();
	}
}
