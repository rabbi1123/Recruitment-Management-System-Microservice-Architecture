using Job.Application.Abstractions.Mapping;
using Job.Application.Features.JobSkill.Queries;
using Job.Domain.JobSkill;

namespace Job.Application.Features.JobSkill
{
	public class JobSkillsMapper : IResponseEntityMapper<JobSkills, JobSkillsResponse>
	{
		public JobSkillsResponse MapToResponse(JobSkills entity)
		{
			return new JobSkillsResponse
			{
				Id = entity.Id,
				JobId = entity.JobId,
				SkillName = entity.SkillName,
				IsRequired = entity.IsRequired,
				IsActive = entity.IsActive
			};
		}
	}
}
