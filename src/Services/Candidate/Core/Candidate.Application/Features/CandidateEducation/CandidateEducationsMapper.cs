using Candidate.Application.Abstractions.Mapping;
using Candidate.Application.Features.CandidateEducation.Queries;
using Candidate.Domain.CandidateEducation;

namespace Candidate.Application.Features.CandidateEducation
{
	public class CandidateEducationsMapper : IResponseEntityMapper<CandidateEducations, CandidateEducationsResponse>
	{
		public CandidateEducationsResponse MapToResponse(CandidateEducations entity)
		{
			return new CandidateEducationsResponse
			{
				Id = entity.Id,
				CandidateId = entity.CandidateId,
				Institution = entity.Institution,
				Degree = entity.Degree,
				FieldOfStudy = entity.FieldOfStudy,
				StartDate = entity.StartDate,
				EndDate = entity.EndDate,
				Grade = entity.Grade,
				IsActive = entity.IsActive
			};
		}
	}
}
