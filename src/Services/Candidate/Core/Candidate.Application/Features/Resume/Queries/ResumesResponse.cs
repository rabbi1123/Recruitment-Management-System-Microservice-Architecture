using Candidate.Application.Common.CRUD.Queries;

namespace Candidate.Application.Features.Resume.Queries
{
	public class ResumesResponse : IGenericResponse
	{
		public long Id { get; set; }
		public long CandidateId { get; set; }
		public string FileName { get; set; }
		public string FileUrl { get; set; }
		public string FileFormat { get; set; }
		public long FileSize { get; set; }
		public bool IsDefault { get; set; }
		public DateTime UploadedOn { get; set; }
		public bool IsActive { get; set; }
	}
}
