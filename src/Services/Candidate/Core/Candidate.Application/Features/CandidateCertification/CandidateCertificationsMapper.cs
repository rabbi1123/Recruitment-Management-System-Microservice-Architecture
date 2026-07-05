using Candidate.Application.Abstractions.Mapping;
using Candidate.Application.Features.CandidateCertification.Queries;
using Candidate.Domain.CandidateCertification;

namespace Candidate.Application.Features.CandidateCertification
{
	public class CandidateCertificationsMapper : IResponseEntityMapper<CandidateCertifications, CandidateCertificationsResponse>
	{
		public CandidateCertificationsResponse MapToResponse(CandidateCertifications entity)
		{
			return new CandidateCertificationsResponse
			{
				Id = entity.Id,
				CandidateId = entity.CandidateId,
				Name = entity.Name,
				IssuingOrg = entity.IssuingOrg,
				IssueDate = entity.IssueDate,
				ExpiryDate = entity.ExpiryDate,
				CredentialId = entity.CredentialId,
				CredentialUrl = entity.CredentialUrl,
				IsActive = entity.IsActive
			};
		}
	}
}
