using Common.Platform.Domain.Abstractions;

namespace Job.Domain.SavedJob
{
	public class SavedJobs : IEntity
	{
		public SavedJobs(
			long id,
			long candidateId,
			long jobId,
			DateTime savedDate,
			bool isActive,
			string? createdBy,
			DateTime? updatedOn,
			string? updatedBy)
		{
			Id = id;
			CandidateId = candidateId;
			JobId = jobId;
			SavedDate = savedDate;
			IsActive = isActive;
			CreatedBy = createdBy;
			UpdatedOn = updatedOn;
			UpdatedBy = updatedBy;
		}

		public SavedJobs()
		{
		}

		public string GetPrimaryKeyName() => nameof(Id);

		public long GetPrimaryKeyValue() => Id;

		public long Id { get; set; }

		public long CandidateId { get; set; }

		public long JobId { get; set; }

		public DateTime SavedDate { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedOn { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public string? UpdatedBy { get; set; }

		public static SavedJobs Create(
			long candidateId,
			long jobId,
			string? createdBy)
		{
			return new SavedJobs(
				id: 0,
				candidateId: candidateId,
				jobId: jobId,
				savedDate: DateTime.Now,
				isActive: true,
				createdBy: createdBy,
				updatedOn: null,
				updatedBy: null);
		}

		public void Update(
			bool isActive,
			string? updatedBy)
		{
			IsActive = isActive;
			UpdatedBy = updatedBy;
			UpdatedOn = DateTime.Now;
		}
	}
}
