using Recruitment.Application.Abstractions.CRUD;
using Recruitment.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Recruitment.Application.Features.ApplicationComment.Commands.UpdateApplicationComments
{
	public record UpdateApplicationCommentsCommand : IRequest<Result<CommandResponse>>, IUpdatedByCommand
	{
		public long Id { get; set; }
		public string Comment { get; set; }
		public bool IsActive { get; set; }
		public string? UpdatedBy { get; set; }
	}
}
