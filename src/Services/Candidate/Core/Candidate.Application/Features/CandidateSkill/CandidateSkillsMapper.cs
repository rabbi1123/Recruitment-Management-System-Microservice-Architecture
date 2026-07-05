using Candidate.Application.Abstractions.Mapping;
using Candidate.Application.Features.CandidateSkill.Queries;
using Candidate.Domain.CandidateSkill;

namespace Candidate.Application.Features.CandidateSkill
{
	public class CandidateSkillsMapper : IResponseEntityMapper<CandidateSkills, CandidateSkillsResponse>
	{
		public CandidateSkillsResponse MapToResponse(CandidateSkills entity)
		{
			return new CandidateSkillsResponse
			{
				Id = entity.Id,
				CandidateId = entity.CandidateId,
				Name = entity.Name,
				ProficiencyLevel = entity.ProficiencyLevel,
				YearsOfExperience = entity.YearsOfExperience,
				IsActive = entity.IsActive
			};
		}
	}
}
