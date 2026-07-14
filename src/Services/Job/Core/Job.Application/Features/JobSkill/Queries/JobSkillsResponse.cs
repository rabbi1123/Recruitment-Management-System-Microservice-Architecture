using Job.Application.Common.CRUD.Queries;

namespace Job.Application.Features.JobSkill.Queries
{
	public class JobSkillsResponse : IGenericResponse
	{
		public long Id { get; set; }
		public long JobId { get; set; }
		public string SkillName { get; set; }
		public bool IsRequired { get; set; }
		public bool IsActive { get; set; }
	}
}
