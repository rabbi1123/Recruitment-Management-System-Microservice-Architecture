using Candidate.Application.Abstractions.CRUD;
using Candidate.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.CandidateSkill.Commands.UpdateCandidateSkills
{
	public record UpdateCandidateSkillsCommand : IRequest<Result<CommandResponse>>, IUpdatedByCommand
	{
		public long Id { get; set; }
		public long CandidateId { get; set; }
		public string Name { get; set; }
		public string? ProficiencyLevel { get; set; }
		public decimal? YearsOfExperience { get; set; }
		public bool IsActive { get; set; }
		public string? UpdatedBy { get; set; }
	}
}
