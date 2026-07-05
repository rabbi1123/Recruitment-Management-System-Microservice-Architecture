using Candidate.Application.Common.CRUD.Queries;

namespace Candidate.Application.Features.CandidateSkill.Queries
{
	public class CandidateSkillsResponse : IGenericResponse
	{
		public long Id { get; set; }
		public long CandidateId { get; set; }
		public string Name { get; set; }
		public string? ProficiencyLevel { get; set; }
		public decimal? YearsOfExperience { get; set; }
		public bool IsActive { get; set; }
	}
}
