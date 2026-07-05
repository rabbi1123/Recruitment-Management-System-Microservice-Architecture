using Common.Platform.Domain.Abstractions;

namespace Candidate.Domain.Resume
{
	public class Resumes : IEntity
	{
		public Resumes(
			long id,
			long candidateId,
			string fileName,
			string fileUrl,
			string fileFormat,
			long fileSize,
			bool isDefault,
			DateTime uploadedOn,
			bool isActive,
			string? createdBy,
			DateTime? updatedOn,
			string? updatedBy)
		{
			Id = id;
			CandidateId = candidateId;
			FileName = fileName;
			FileUrl = fileUrl;
			FileFormat = fileFormat;
			FileSize = fileSize;
			IsDefault = isDefault;
			UploadedOn = uploadedOn;
			IsActive = isActive;

			CreatedBy = createdBy;
			UpdatedOn = updatedOn;
			UpdatedBy = updatedBy;
		}

		public Resumes()
		{
		}

		public string GetPrimaryKeyName() => nameof(Id);

		public long GetPrimaryKeyValue() => Id;

		public long Id { get; set; }

		public long CandidateId { get; set; }

		public string FileName { get; set; }

		public string FileUrl { get; set; }

		public string FileFormat { get; set; }

		public long FileSize { get; set; }

		public bool IsDefault { get; set; }

		public DateTime UploadedOn { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedOn { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public string? UpdatedBy { get; set; }

		public static Resumes Create(
			long candidateId,
			string fileName,
			string fileUrl,
			string fileFormat,
			long fileSize,
			bool isDefault,
			string? createdBy)
		{
			return new Resumes(
				id: 0,
				candidateId: candidateId,
				fileName: fileName,
				fileUrl: fileUrl,
				fileFormat: fileFormat,
				fileSize: fileSize,
				isDefault: isDefault,
				uploadedOn: DateTime.Now,
				isActive: true,
				createdBy: createdBy,
				updatedOn: null,
				updatedBy: null);
		}

		public void Update(
			string fileName,
			string fileUrl,
			string fileFormat,
			long fileSize,
			bool isDefault,
			bool isActive,
			string? updatedBy)
		{
			FileName = fileName;
			FileUrl = fileUrl;
			FileFormat = fileFormat;
			FileSize = fileSize;
			IsDefault = isDefault;
			IsActive = isActive;

			UpdatedBy = updatedBy;
			UpdatedOn = DateTime.Now;
		}
	}
}
