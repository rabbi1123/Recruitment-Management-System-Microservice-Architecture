using Candidate.Application.Abstractions.Mapping;
using Candidate.Application.Features.CandidateExperience.Queries;
using Candidate.Domain.CandidateExperience;

namespace Candidate.Application.Features.CandidateExperience
{
	public class CandidateExperiencesMapper : IResponseEntityMapper<CandidateExperiences, CandidateExperiencesResponse>
	{
		public CandidateExperiencesResponse MapToResponse(CandidateExperiences entity)
		{
			return new CandidateExperiencesResponse
			{
				Id = entity.Id,
				CandidateId = entity.CandidateId,
				Company = entity.Company,
				Title = entity.Title,
				Location = entity.Location,
				StartDate = entity.StartDate,
				EndDate = entity.EndDate,
				IsCurrent = entity.IsCurrent,
				Description = entity.Description,
				IsActive = entity.IsActive
			};
		}
	}
}
