using Job.Application.Abstractions.CRUD;
using Job.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Job.Application.Features.SavedJob.Commands.AddSavedJobs
{
	public record AddSavedJobsCommand : IRequest<Result<CommandResponse>>, ICreatedByCommand
	{
		public long CandidateId { get; set; }
		public long JobId { get; set; }
		public string? CreatedBy { get; set; }
	}
}
