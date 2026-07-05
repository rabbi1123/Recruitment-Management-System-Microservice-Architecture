using Common.Platform.Domain.Abstractions;

namespace Candidate.Domain.CandidateEducation
{
	public class CandidateEducations : IEntity
	{
		public CandidateEducations(
			long id,
			long candidateId,
			string institution,
			string? degree,
			string? fieldOfStudy,
			DateOnly? startDate,
			DateOnly? endDate,
			string? grade,
			bool isActive,
			string? createdBy,
			DateTime? updatedOn,
			string? updatedBy)
		{
			Id = id;
			CandidateId = candidateId;
			Institution = institution;
			Degree = degree;
			FieldOfStudy = fieldOfStudy;
			StartDate = startDate;
			EndDate = endDate;
			Grade = grade;
			IsActive = isActive;

			CreatedBy = createdBy;
			UpdatedOn = updatedOn;
			UpdatedBy = updatedBy;
		}

		public CandidateEducations()
		{
		}

		public string GetPrimaryKeyName() => nameof(Id);

		public long GetPrimaryKeyValue() => Id;

		public long Id { get; set; }

		public long CandidateId { get; set; }

		public string Institution { get; set; }

		public string? Degree { get; set; }

		public string? FieldOfStudy { get; set; }

		public DateOnly? StartDate { get; set; }

		public DateOnly? EndDate { get; set; }

		public string? Grade { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedOn { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public string? UpdatedBy { get; set; }

		public static CandidateEducations Create(
			long candidateId,
			string institution,
			string? degree,
			string? fieldOfStudy,
			DateOnly? startDate,
			DateOnly? endDate,
			string? grade,
			string? createdBy)
		{
			return new CandidateEducations(
				id: 0,
				candidateId: candidateId,
				institution: institution,
				degree: degree,
				fieldOfStudy: fieldOfStudy,
				startDate: startDate,
				endDate: endDate,
				grade: grade,
				isActive: true,
				createdBy: createdBy,
				updatedOn: null,
				updatedBy: null);
		}

		public void Update(
			string institution,
			string? degree,
			string? fieldOfStudy,
			DateOnly? startDate,
			DateOnly? endDate,
			string? grade,
			bool isActive,
			string? updatedBy)
		{
			Institution = institution;
			Degree = degree;
			FieldOfStudy = fieldOfStudy;
			StartDate = startDate;
			EndDate = endDate;
			Grade = grade;
			IsActive = isActive;

			UpdatedBy = updatedBy;
			UpdatedOn = DateTime.Now;
		}
	}
}
