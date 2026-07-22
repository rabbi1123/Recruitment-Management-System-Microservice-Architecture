using Recruitment.Application.Abstractions.CRUD;
using Recruitment.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Recruitment.Application.Features.Application.Commands.AddApplications
{
	public record AddApplicationsCommand : IRequest<Result<CommandResponse>>, ICreatedByCommand
	{
		public long JobId { get; set; }
		public long CandidateId { get; set; }
		public long OrganizationId { get; set; }
		public long? RecruiterId { get; set; }
		public string? CoverLetter { get; set; }
		public string? CreatedBy { get; set; }
	}
}
