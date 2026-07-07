using Candidate.Application.Abstractions.CRUD;
using Candidate.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.CandidateExperience.Commands.UpdateCandidateExperiences
{
	public record UpdateCandidateExperiencesCommand : IRequest<Result<CommandResponse>>, IUpdatedByCommand
	{
		public long Id { get; set; }
		public long CandidateId { get; set; }
		public string Company { get; set; }
		public string Title { get; set; }
		public string? Location { get; set; }
		public DateOnly StartDate { get; set; }
		public DateOnly? EndDate { get; set; }
		public bool IsCurrent { get; set; }
		public string? Description { get; set; }
		public bool IsActive { get; set; }
		public string? UpdatedBy { get; set; }
	}
}
