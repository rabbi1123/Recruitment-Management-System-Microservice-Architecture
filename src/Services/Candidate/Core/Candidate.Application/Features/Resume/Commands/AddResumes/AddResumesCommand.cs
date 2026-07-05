using Candidate.Application.Abstractions.CRUD;
using Candidate.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.Resume.Commands.AddResumes
{
	public record AddResumesCommand : IRequest<Result<CommandResponse>>, ICreatedByCommand
	{
		public long CandidateId { get; set; }
		public string FileName { get; set; }
		public string FileUrl { get; set; }
		public string FileFormat { get; set; }
		public long FileSize { get; set; }
		public bool IsDefault { get; set; }
		public string? CreatedBy { get; set; }
	}
}
