using Job.Application.Common.CRUD.Queries;

namespace Job.Application.Features.Job.Queries
{
	public class JobsResponse : IGenericResponse
	{
		public long Id { get; set; }
		public long OrganizationId { get; set; }
		public long RecruiterId { get; set; }
		public string Title { get; set; }
		public string? Department { get; set; }
		public string EmploymentType { get; set; }
		public string? Location { get; set; }
		public bool IsRemote { get; set; }
		public int? ExperienceMin { get; set; }
		public int? ExperienceMax { get; set; }
		public decimal? SalaryMin { get; set; }
		public decimal? SalaryMax { get; set; }
		public string? Currency { get; set; }
		public string? Description { get; set; }
		public string Status { get; set; }
		public DateTime? PublishedDate { get; set; }
		public DateTime? ClosedDate { get; set; }
		public bool IsActive { get; set; }
	}
}
