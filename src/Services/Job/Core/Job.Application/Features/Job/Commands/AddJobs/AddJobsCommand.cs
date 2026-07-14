using Job.Application.Abstractions.CRUD;
using Job.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Job.Application.Features.Job.Commands.AddJobs
{
	public record AddJobsCommand : IRequest<Result<CommandResponse>>, ICreatedByCommand
	{
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
		public string? CreatedBy { get; set; }
	}
}
