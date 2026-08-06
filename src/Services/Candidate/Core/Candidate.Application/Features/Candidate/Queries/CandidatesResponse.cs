using Candidate.Application.Common.CRUD.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candidate.Application.Features.Candidate.Queries
{
	public class CandidatesResponse : IGenericResponse
	{
		public long Id { get; set; }
		public Guid UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string? Phone { get; set; }
		public string? Address { get; set; }
		public string? LinkedinProfile { get; set; }
		public string? PortfolioUrl { get; set; }
		public string? Headline { get; set; }
		public string? Summary { get; set; }
		public bool IsActive { get; set; }
	}
}
