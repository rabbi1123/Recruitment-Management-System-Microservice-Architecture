using Common.Platform.Domain.Abstractions;

namespace Recruitment.Domain.ApplicationComment
{
	public class ApplicationComments : IEntity
	{
		public ApplicationComments(
			long id,
			long applicationId,
			long authorId,
			string comment,
			bool isActive,
			string? createdBy,
			DateTime? updatedOn,
			string? updatedBy)
		{
			Id = id;
			ApplicationId = applicationId;
			AuthorId = authorId;
			Comment = comment;
			IsActive = isActive;

			CreatedBy = createdBy;
			UpdatedOn = updatedOn;
			UpdatedBy = updatedBy;
		}

		public ApplicationComments()
		{
		}

		public string GetPrimaryKeyName() => nameof(Id);

		public long GetPrimaryKeyValue() => Id;

		public long Id { get; set; }

		public long ApplicationId { get; set; }

		public long AuthorId { get; set; }

		public string Comment { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedOn { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public string? UpdatedBy { get; set; }

		public static ApplicationComments Create(
			long applicationId,
			long authorId,
			string comment,
			string? createdBy)
		{
			return new ApplicationComments(
				id: 0,
				applicationId: applicationId,
				authorId: authorId,
				comment: comment,
				isActive: true,
				createdBy: createdBy,
				updatedOn: null,
				updatedBy: null);
		}

		public void Update(
			string comment,
			bool isActive,
			string? updatedBy)
		{
			Comment = comment;
			IsActive = isActive;

			UpdatedBy = updatedBy;
			UpdatedOn = DateTime.Now;
		}
	}
}
