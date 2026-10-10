using Common.Platform.Domain.Abstractions;
using Notification.Application.Abstractions.CRUD;

namespace Notification.Application.Common.CRUD.Queries;

public sealed record GetAllQuery<TResponse> : IGenericRequest<Result<List<TResponse>>>
{
	public SortOption? SortBy { get; init; }
}
