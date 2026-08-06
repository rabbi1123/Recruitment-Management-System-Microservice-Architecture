using Candidate.Application.Abstractions.CRUD;
using Candidate.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candidate.Application.Features.Candidate.Commands.AddCandidates
{
	public record AddCandidatesCommand : IRequest<Result<CommandResponse>>, ICreatedByCommand
	{
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
		public string? CreatedBy { get; set; }
	}
}
