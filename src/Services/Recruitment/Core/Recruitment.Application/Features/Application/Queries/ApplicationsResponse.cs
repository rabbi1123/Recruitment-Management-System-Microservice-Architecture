using Recruitment.Application.Common.CRUD.Queries;

namespace Recruitment.Application.Features.Application.Queries
{
	public class ApplicationsResponse : IGenericResponse
	{
		public long Id { get; set; }
		public long JobId { get; set; }
		public long CandidateId { get; set; }
		public long OrganizationId { get; set; }
		public long? RecruiterId { get; set; }
		public string CurrentStage { get; set; }
		public string Status { get; set; }
		public string? CoverLetter { get; set; }
		public string? RejectionReason { get; set; }
		public bool IsActive { get; set; }
	}
}
