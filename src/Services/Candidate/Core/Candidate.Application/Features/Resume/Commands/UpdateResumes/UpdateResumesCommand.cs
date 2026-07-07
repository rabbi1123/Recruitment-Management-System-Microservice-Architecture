using Candidate.Application.Abstractions.CRUD;
using Candidate.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.Resume.Commands.UpdateResumes
{
	public record UpdateResumesCommand : IRequest<Result<CommandResponse>>, IUpdatedByCommand
	{
		public long Id { get; set; }
		public long CandidateId { get; set; }
		public string FileName { get; set; }
		public string FileUrl { get; set; }
		public string FileFormat { get; set; }
		public long FileSize { get; set; }
		public bool IsDefault { get; set; }
		public bool IsActive { get; set; }
		public string? UpdatedBy { get; set; }
	}
}
