using Candidate.Application.Abstractions.CRUD;
using Candidate.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.CandidateCertification.Commands.UpdateCandidateCertifications
{
	public record UpdateCandidateCertificationsCommand : IRequest<Result<CommandResponse>>, IUpdatedByCommand
	{
		public long Id { get; set; }
		public long CandidateId { get; set; }
		public string Name { get; set; }
		public string? IssuingOrg { get; set; }
		public DateOnly? IssueDate { get; set; }
		public DateOnly? ExpiryDate { get; set; }
		public string? CredentialId { get; set; }
		public string? CredentialUrl { get; set; }
		public bool IsActive { get; set; }
		public string? UpdatedBy { get; set; }
	}
}
