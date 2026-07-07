using Candidate.Application.Common.CRUD.Queries;

namespace Candidate.Application.Features.CandidateExperience.Queries
{
	public class CandidateExperiencesResponse : IGenericResponse
	{
		public long Id { get; set; }
		public long CandidateId { get; set; }
		public string Company { get; set; }
		public string Title { get; set; }
		public string? Location { get; set; }
		public DateOnly StartDate { get; set; }
		public DateOnly? EndDate { get; set; }
		public bool IsCurrent { get; set; }
		public string? Description { get; set; }
		public bool IsActive { get; set; }
	}
}
