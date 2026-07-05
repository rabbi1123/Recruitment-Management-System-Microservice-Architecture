using Common.Platform.Domain.Abstractions;

namespace Candidate.Domain.CandidateSkill
{
	public class CandidateSkills : IEntity
	{
		public CandidateSkills(
			long id,
			long candidateId,
			string name,
			string? proficiencyLevel,
			decimal? yearsOfExperience,
			bool isActive,
			string? createdBy,
			DateTime? updatedOn,
			string? updatedBy)
		{
			Id = id;
			CandidateId = candidateId;
			Name = name;
			ProficiencyLevel = proficiencyLevel;
			YearsOfExperience = yearsOfExperience;
			IsActive = isActive;

			CreatedBy = createdBy;
			UpdatedOn = updatedOn;
			UpdatedBy = updatedBy;
		}

		public CandidateSkills()
		{
		}

		public string GetPrimaryKeyName() => nameof(Id);

		public long GetPrimaryKeyValue() => Id;

		public long Id { get; set; }

		public long CandidateId { get; set; }

		public string Name { get; set; }

		public string? ProficiencyLevel { get; set; }

		public decimal? YearsOfExperience { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedOn { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public string? UpdatedBy { get; set; }

		public static CandidateSkills Create(
			long candidateId,
			string name,
			string? proficiencyLevel,
			decimal? yearsOfExperience,
			string? createdBy)
		{
			return new CandidateSkills(
				id: 0,
				candidateId: candidateId,
				name: name,
				proficiencyLevel: proficiencyLevel,
				yearsOfExperience: yearsOfExperience,
				isActive: true,
				createdBy: createdBy,
				updatedOn: null,
				updatedBy: null);
		}

		public void Update(
			string name,
			string? proficiencyLevel,
			decimal? yearsOfExperience,
			bool isActive,
			string? updatedBy)
		{
			Name = name;
			ProficiencyLevel = proficiencyLevel;
			YearsOfExperience = yearsOfExperience;
			IsActive = isActive;

			UpdatedBy = updatedBy;
			UpdatedOn = DateTime.Now;
		}
	}
}
