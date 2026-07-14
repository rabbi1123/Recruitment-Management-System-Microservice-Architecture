using Job.Application.Common.CRUD.Queries;

namespace Job.Application.Features.SavedJob.Queries
{
	public class SavedJobsResponse : IGenericResponse
	{
		public long Id { get; set; }
		public long CandidateId { get; set; }
		public long JobId { get; set; }
		public DateTime SavedDate { get; set; }
		public bool IsActive { get; set; }
	}
}
