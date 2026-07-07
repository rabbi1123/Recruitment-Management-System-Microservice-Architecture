using Candidate.Application.Abstractions.Mapping;
using Candidate.Application.Features.Resume.Queries;
using Candidate.Domain.Resume;

namespace Candidate.Application.Features.Resume
{
	public class ResumesMapper : IResponseEntityMapper<Resumes, ResumesResponse>
	{
		public ResumesResponse MapToResponse(Resumes entity)
		{
			return new ResumesResponse
			{
				Id = entity.Id,
				CandidateId = entity.CandidateId,
				FileName = entity.FileName,
				FileUrl = entity.FileUrl,
				FileFormat = entity.FileFormat,
				FileSize = entity.FileSize,
				IsDefault = entity.IsDefault,
				UploadedOn = entity.UploadedOn,
				IsActive = entity.IsActive
			};
		}
	}
}
