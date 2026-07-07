using Candidate.Application.Abstractions.CRUD;
using Candidate.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.CandidateCertification.Commands.AddCandidateCertifications
{
	public record AddCandidateCertificationsCommand : IRequest<Result<CommandResponse>>, ICreatedByCommand
	{
		public long CandidateId { get; set; }
		public string Name { get; set; }
		public string? IssuingOrg { get; set; }
		public DateOnly? IssueDate { get; set; }
		public DateOnly? ExpiryDate { get; set; }
		public string? CredentialId { get; set; }
		public string? CredentialUrl { get; set; }
		public string? CreatedBy { get; set; }
	}
}
