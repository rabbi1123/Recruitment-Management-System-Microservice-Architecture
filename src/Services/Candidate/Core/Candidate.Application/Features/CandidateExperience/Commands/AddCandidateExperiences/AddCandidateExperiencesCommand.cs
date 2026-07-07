using Candidate.Application.Abstractions.CRUD;
using Candidate.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.CandidateExperience.Commands.AddCandidateExperiences
{
	public record AddCandidateExperiencesCommand : IRequest<Result<CommandResponse>>, ICreatedByCommand
	{
		public long CandidateId { get; set; }
		public string Company { get; set; }
		public string Title { get; set; }
		public string? Location { get; set; }
		public DateOnly StartDate { get; set; }
		public DateOnly? EndDate { get; set; }
		public bool IsCurrent { get; set; }
		public string? Description { get; set; }
		public string? CreatedBy { get; set; }
	}
}
