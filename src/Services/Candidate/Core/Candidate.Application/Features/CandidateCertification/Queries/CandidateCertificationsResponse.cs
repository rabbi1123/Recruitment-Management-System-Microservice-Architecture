using Candidate.Application.Common.CRUD.Queries;

namespace Candidate.Application.Features.CandidateCertification.Queries
{
	public class CandidateCertificationsResponse : IGenericResponse
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
	}
}
