using Common.Platform.Domain.Abstractions;

namespace Job.Domain.JobSkill
{
	public class JobSkills : IEntity
	{
		public JobSkills(
			long id,
			long jobId,
			string skillName,
			bool isRequired,
			bool isActive,
			string? createdBy,
			DateTime? updatedOn,
			string? updatedBy)
		{
			Id = id;
			JobId = jobId;
			SkillName = skillName;
			IsRequired = isRequired;
			IsActive = isActive;
			CreatedBy = createdBy;
			UpdatedOn = updatedOn;
			UpdatedBy = updatedBy;
		}

		public JobSkills()
		{
		}

		public string GetPrimaryKeyName() => nameof(Id);

		public long GetPrimaryKeyValue() => Id;

		public long Id { get; set; }

		public long JobId { get; set; }

		public string SkillName { get; set; }

		public bool IsRequired { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedOn { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public string? UpdatedBy { get; set; }

		public static JobSkills Create(
			long jobId,
			string skillName,
			bool isRequired,
			string? createdBy)
		{
			return new JobSkills(
				id: 0,
				jobId: jobId,
				skillName: skillName,
				isRequired: isRequired,
				isActive: true,
				createdBy: createdBy,
				updatedOn: null,
				updatedBy: null);
		}

		public void Update(
			string skillName,
			bool isRequired,
			bool isActive,
			string? updatedBy)
		{
			SkillName = skillName;
			IsRequired = isRequired;
			IsActive = isActive;
			UpdatedBy = updatedBy;
			UpdatedOn = DateTime.Now;
		}
	}
}
