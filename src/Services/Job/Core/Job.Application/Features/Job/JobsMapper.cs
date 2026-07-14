using Job.Application.Abstractions.Mapping;
using Job.Application.Features.Job.Queries;
using Job.Domain.Job;

namespace Job.Application.Features.Job
{
	public class JobsMapper : IResponseEntityMapper<Jobs, JobsResponse>
	{
		public JobsResponse MapToResponse(Jobs entity)
		{
			return new JobsResponse
			{
				Id = entity.Id,
				OrganizationId = entity.OrganizationId,
				RecruiterId = entity.RecruiterId,
				Title = entity.Title,
				Department = entity.Department,
				EmploymentType = entity.EmploymentType,
				Location = entity.Location,
				IsRemote = entity.IsRemote,
				ExperienceMin = entity.ExperienceMin,
				ExperienceMax = entity.ExperienceMax,
				SalaryMin = entity.SalaryMin,
				SalaryMax = entity.SalaryMax,
				Currency = entity.Currency,
				Description = entity.Description,
				Status = entity.Status,
				PublishedDate = entity.PublishedDate,
				ClosedDate = entity.ClosedDate,
				IsActive = entity.IsActive
			};
		}
	}
}
