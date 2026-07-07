using Common.Platform.Domain.Abstractions;

namespace Candidate.Domain.CandidateExperience
{
	public class CandidateExperiences : IEntity
	{
		public CandidateExperiences(
			long id,
			long candidateId,
			string company,
			string title,
			string? location,
			DateOnly startDate,
			DateOnly? endDate,
			bool isCurrent,
			string? description,
			bool isActive,
			string? createdBy,
			DateTime? updatedOn,
			string? updatedBy)
		{
			Id = id;
			CandidateId = candidateId;
			Company = company;
			Title = title;
			Location = location;
			StartDate = startDate;
			EndDate = endDate;
			IsCurrent = isCurrent;
			Description = description;
			IsActive = isActive;

			CreatedBy = createdBy;
			UpdatedOn = updatedOn;
			UpdatedBy = updatedBy;
		}

		public CandidateExperiences()
		{
		}

		public string GetPrimaryKeyName() => nameof(Id);

		public long GetPrimaryKeyValue() => Id;

		public long Id { get; set; }

		public long CandidateId { get; set; }

		public string Company { get; set; }

		public string Title { get; set; }

		public string? Location { get; set; }

		public DateOnly StartDate { get; set; }

		public DateOnly? EndDate { get; set; }

		public bool IsCurrent { get; set; }

		public string? Description { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedOn { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public string? UpdatedBy { get; set; }

		public static CandidateExperiences Create(
			long candidateId,
			string company,
			string title,
			string? location,
			DateOnly startDate,
			DateOnly? endDate,
			bool isCurrent,
			string? description,
			string? createdBy)
		{
			return new CandidateExperiences(
				id: 0,
				candidateId: candidateId,
				company: company,
				title: title,
				location: location,
				startDate: startDate,
				endDate: endDate,
				isCurrent: isCurrent,
				description: description,
				isActive: true,
				createdBy: createdBy,
				updatedOn: null,
				updatedBy: null);
		}

		public void Update(
			string company,
			string title,
			string? location,
			DateOnly startDate,
			DateOnly? endDate,
			bool isCurrent,
			string? description,
			bool isActive,
			string? updatedBy)
		{
			Company = company;
			Title = title;
			Location = location;
			StartDate = startDate;
			EndDate = endDate;
			IsCurrent = isCurrent;
			Description = description;
			IsActive = isActive;

			UpdatedBy = updatedBy;
			UpdatedOn = DateTime.Now;
		}
	}
}
