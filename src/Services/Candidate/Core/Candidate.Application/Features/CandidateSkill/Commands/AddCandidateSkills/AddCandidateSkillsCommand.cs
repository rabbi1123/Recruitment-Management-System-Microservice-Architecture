using Candidate.Application.Abstractions.CRUD;
using Candidate.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.CandidateSkill.Commands.AddCandidateSkills
{
	public record AddCandidateSkillsCommand : IRequest<Result<CommandResponse>>, ICreatedByCommand
	{
		public long CandidateId { get; set; }
		public string Name { get; set; }
		public string? ProficiencyLevel { get; set; }
		public decimal? YearsOfExperience { get; set; }
		public string? CreatedBy { get; set; }
	}
}
