using Recruitment.Application.Abstractions.CRUD;
using Recruitment.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Recruitment.Application.Features.Application.Commands.UpdateApplications
{
	public record UpdateApplicationsCommand : IRequest<Result<CommandResponse>>, IUpdatedByCommand
	{
		public long Id { get; set; }
		public long? RecruiterId { get; set; }
		public string CurrentStage { get; set; }
		public string Status { get; set; }
		public string? CoverLetter { get; set; }
		public string? RejectionReason { get; set; }
		public bool IsActive { get; set; }
		public string? UpdatedBy { get; set; }
	}
}
