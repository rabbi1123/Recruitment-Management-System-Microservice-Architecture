using Common.Platform.Domain.Abstractions;

namespace Job.Domain.Job
{
	public class Jobs : IEntity
	{
		public Jobs(
			long id,
			long organizationId,
			long recruiterId,
			string title,
			string? department,
			string employmentType,
			string? location,
			bool isRemote,
			int? experienceMin,
			int? experienceMax,
			decimal? salaryMin,
			decimal? salaryMax,
			string? currency,
			string? description,
			string status,
			DateTime? publishedDate,
			DateTime? closedDate,
			bool isActive,
			string? createdBy,
			DateTime? updatedOn,
			string? updatedBy)
		{
			Id = id;
			OrganizationId = organizationId;
			RecruiterId = recruiterId;
			Title = title;
			Department = department;
			EmploymentType = employmentType;
			Location = location;
			IsRemote = isRemote;
			ExperienceMin = experienceMin;
			ExperienceMax = experienceMax;
			SalaryMin = salaryMin;
			SalaryMax = salaryMax;
			Currency = currency;
			Description = description;
			Status = status;
			PublishedDate = publishedDate;
			ClosedDate = closedDate;
			IsActive = isActive;
			CreatedBy = createdBy;
			UpdatedOn = updatedOn;
			UpdatedBy = updatedBy;
		}

		public Jobs()
		{
		}

		public string GetPrimaryKeyName() => nameof(Id);

		public long GetPrimaryKeyValue() => Id;

		public long Id { get; set; }

		public long OrganizationId { get; set; }

		public long RecruiterId { get; set; }

		public string Title { get; set; }

		public string? Department { get; set; }

		public string EmploymentType { get; set; }

		public string? Location { get; set; }

		public bool IsRemote { get; set; }

		public int? ExperienceMin { get; set; }

		public int? ExperienceMax { get; set; }

		public decimal? SalaryMin { get; set; }

		public decimal? SalaryMax { get; set; }

		public string? Currency { get; set; }

		public string? Description { get; set; }

		public string Status { get; set; }

		public DateTime? PublishedDate { get; set; }

		public DateTime? ClosedDate { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedOn { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public string? UpdatedBy { get; set; }

		public static Jobs Create(
			long organizationId,
			long recruiterId,
			string title,
			string? department,
			string employmentType,
			string? location,
			bool isRemote,
			int? experienceMin,
			int? experienceMax,
			decimal? salaryMin,
			decimal? salaryMax,
			string? currency,
			string? description,
			string status,
			string? createdBy)
		{
			var now = DateTime.Now;
			DateTime? publishedDate = status == "Published" ? now : null;
			DateTime? closedDate = status == "Closed" ? now : null;

			return new Jobs(
				id: 0,
				organizationId: organizationId,
				recruiterId: recruiterId,
				title: title,
				department: department,
				employmentType: employmentType,
				location: location,
				isRemote: isRemote,
				experienceMin: experienceMin,
				experienceMax: experienceMax,
				salaryMin: salaryMin,
				salaryMax: salaryMax,
				currency: currency,
				description: description,
				status: status,
				publishedDate: publishedDate,
				closedDate: closedDate,
				isActive: true,
				createdBy: createdBy,
				updatedOn: null,
				updatedBy: null);
		}

		public void Update(
			long organizationId,
			long recruiterId,
			string title,
			string? department,
			string employmentType,
			string? location,
			bool isRemote,
			int? experienceMin,
			int? experienceMax,
			decimal? salaryMin,
			decimal? salaryMax,
			string? currency,
			string? description,
			string status,
			bool isActive,
			string? updatedBy)
		{
			var previousStatus = Status;

			OrganizationId = organizationId;
			RecruiterId = recruiterId;
			Title = title;
			Department = department;
			EmploymentType = employmentType;
			Location = location;
			IsRemote = isRemote;
			ExperienceMin = experienceMin;
			ExperienceMax = experienceMax;
			SalaryMin = salaryMin;
			SalaryMax = salaryMax;
			Currency = currency;
			Description = description;
			Status = status;
			IsActive = isActive;

			if (status == "Published" && previousStatus != "Published")
			{
				PublishedDate = DateTime.Now;
			}

			if (status == "Closed" && previousStatus != "Closed")
			{
				ClosedDate = DateTime.Now;
			}

			UpdatedBy = updatedBy;
			UpdatedOn = DateTime.Now;
		}
	}
}
