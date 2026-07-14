using Job.Application.Abstractions.CRUD;
using Job.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Job.Application.Features.SavedJob.Commands.UpdateSavedJobs
{
	public record UpdateSavedJobsCommand : IRequest<Result<CommandResponse>>, IUpdatedByCommand
	{
		public long Id { get; set; }
		public long CandidateId { get; set; }
		public long JobId { get; set; }
		public bool IsActive { get; set; }
		public string? UpdatedBy { get; set; }
	}
}
