using Job.Application.Abstractions.Mapping;
using Job.Application.Features.SavedJob.Queries;
using Job.Domain.SavedJob;

namespace Job.Application.Features.SavedJob
{
	public class SavedJobsMapper : IResponseEntityMapper<SavedJobs, SavedJobsResponse>
	{
		public SavedJobsResponse MapToResponse(SavedJobs entity)
		{
			return new SavedJobsResponse
			{
				Id = entity.Id,
				CandidateId = entity.CandidateId,
				JobId = entity.JobId,
				SavedDate = entity.SavedDate,
				IsActive = entity.IsActive
			};
		}
	}
}
