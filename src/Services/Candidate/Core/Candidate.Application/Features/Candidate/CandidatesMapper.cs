using Candidate.Application.Abstractions.Mapping;
using Candidate.Application.Features.Candidate.Queries;
using Candidate.Domain.Candidate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Candidate.Application.Features.Candidate
{
	public class CandidatesMapper : IResponseEntityMapper<Candidates, CandidatesResponse>
	{
		public CandidatesResponse MapToResponse(Candidates entity)
		{
			return new CandidatesResponse
			{
				Id = entity.Id,
				UserId = entity.UserId,
				FirstName = entity.FirstName,
				LastName = entity.LastName,
				Email = entity.Email,
				Phone = entity.Phone,
				Address = entity.Address,
				LinkedinProfile = entity.LinkedinProfile,
				PortfolioUrl = entity.PortfolioUrl,
				Headline = entity.Headline,
				Summary = entity.Summary,
				IsActive = entity.IsActive
			};
		}
	}
}
