using Recruitment.Application.Abstractions.Mapping;
using Recruitment.Application.Features.Application.Queries;
using Recruitment.Domain.Application;

namespace Recruitment.Application.Features.Application
{
	public class ApplicationsMapper : IResponseEntityMapper<Applications, ApplicationsResponse>
	{
		public ApplicationsResponse MapToResponse(Applications entity)
		{
			return new ApplicationsResponse
			{
				Id = entity.Id,
				JobId = entity.JobId,
				CandidateId = entity.CandidateId,
				OrganizationId = entity.OrganizationId,
				RecruiterId = entity.RecruiterId,
				CurrentStage = entity.CurrentStage,
				Status = entity.Status,
				CoverLetter = entity.CoverLetter,
				RejectionReason = entity.RejectionReason,
				IsActive = entity.IsActive
			};
		}
	}
}
