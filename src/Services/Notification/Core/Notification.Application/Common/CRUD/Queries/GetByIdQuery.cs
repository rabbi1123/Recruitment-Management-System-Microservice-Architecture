using Common.Platform.Domain.Abstractions;
using Notification.Application.Abstractions.CRUD;
using System.ComponentModel.DataAnnotations;

namespace Notification.Application.Common.CRUD.Queries;

public sealed record GetByIdQuery<TResponse> : IGenericRequest<Result<TResponse>>
{
	[Required(ErrorMessage = "id is required.")]
	[Range(1, long.MaxValue, ErrorMessage = "id must be greater than 0.")]
	public long Id { get; set; }
}
