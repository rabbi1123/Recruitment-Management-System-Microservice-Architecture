using Candidate.Application.Common.CRUD.Queries;

namespace Candidate.Application.Features.CandidateEducation.Queries
{
	public class CandidateEducationsResponse : IGenericResponse
	{
		public long Id { get; set; }
		public long CandidateId { get; set; }
		public string Institution { get; set; }
		public string? Degree { get; set; }
		public string? FieldOfStudy { get; set; }
		public DateOnly? StartDate { get; set; }
		public DateOnly? EndDate { get; set; }
		public string? Grade { get; set; }
		public bool IsActive { get; set; }
	}
}
