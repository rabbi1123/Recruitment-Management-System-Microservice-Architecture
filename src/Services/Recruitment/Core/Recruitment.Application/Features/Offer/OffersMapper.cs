using Recruitment.Application.Abstractions.Mapping;
using Recruitment.Application.Features.Offer.Queries;
using Recruitment.Domain.Offer;

namespace Recruitment.Application.Features.Offer
{
	public class OffersMapper : IResponseEntityMapper<Offers, OffersResponse>
	{
		public OffersResponse MapToResponse(Offers entity)
		{
			return new OffersResponse
			{
				Id = entity.Id,
				ApplicationId = entity.ApplicationId,
				CandidateId = entity.CandidateId,
				JobId = entity.JobId,
				OrganizationId = entity.OrganizationId,
				Position = entity.Position,
				Salary = entity.Salary,
				Currency = entity.Currency,
				JoiningDate = entity.JoiningDate,
				Benefits = entity.Benefits,
				ExpirationDate = entity.ExpirationDate,
				Status = entity.Status,
				SentDate = entity.SentDate,
				RespondedDate = entity.RespondedDate,
				IsActive = entity.IsActive
			};
		}
	}
}
