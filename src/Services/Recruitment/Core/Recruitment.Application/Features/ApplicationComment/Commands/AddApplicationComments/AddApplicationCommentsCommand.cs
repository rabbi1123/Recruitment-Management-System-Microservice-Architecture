using Recruitment.Application.Abstractions.CRUD;
using Recruitment.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Recruitment.Application.Features.ApplicationComment.Commands.AddApplicationComments
{
	public record AddApplicationCommentsCommand : IRequest<Result<CommandResponse>>, ICreatedByCommand
	{
		public long ApplicationId { get; set; }
		public long AuthorId { get; set; }
		public string Comment { get; set; }
		public string? CreatedBy { get; set; }
	}
}
