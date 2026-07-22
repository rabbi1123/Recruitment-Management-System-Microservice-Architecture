using Common.Platform.Domain.Abstractions;

namespace Recruitment.Domain.Offer
{
	public class Offers : IEntity
	{
		public static readonly string[] AllowedStatuses =
		{
			"Draft",
			"Sent",
			"Accepted",
			"Rejected",
			"Expired"
		};

		public Offers(
			long id,
			long applicationId,
			long candidateId,
			long jobId,
			long organizationId,
			string position,
			decimal salary,
			string currency,
			DateTime? joiningDate,
			string? benefits,
			DateTime? expirationDate,
			string status,
			DateTime? sentDate,
			DateTime? respondedDate,
			bool isActive,
			string? createdBy,
			DateTime? updatedOn,
			string? updatedBy)
		{
			Id = id;
			ApplicationId = applicationId;
			CandidateId = candidateId;
			JobId = jobId;
			OrganizationId = organizationId;
			Position = position;
			Salary = salary;
			Currency = currency;
			JoiningDate = joiningDate;
			Benefits = benefits;
			ExpirationDate = expirationDate;
			Status = status;
			SentDate = sentDate;
			RespondedDate = respondedDate;
			IsActive = isActive;

			CreatedBy = createdBy;
			UpdatedOn = updatedOn;
			UpdatedBy = updatedBy;
		}

		public Offers()
		{
		}

		public string GetPrimaryKeyName() => nameof(Id);

		public long GetPrimaryKeyValue() => Id;

		public long Id { get; set; }

		public long ApplicationId { get; set; }

		public long CandidateId { get; set; }

		public long JobId { get; set; }

		public long OrganizationId { get; set; }

		public string Position { get; set; }

		public decimal Salary { get; set; }

		public string Currency { get; set; }

		public DateTime? JoiningDate { get; set; }

		public string? Benefits { get; set; }

		public DateTime? ExpirationDate { get; set; }

		public string Status { get; set; }

		public DateTime? SentDate { get; set; }

		public DateTime? RespondedDate { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedOn { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public string? UpdatedBy { get; set; }

		public static Offers Create(
			long applicationId,
			long candidateId,
			long jobId,
			long organizationId,
			string position,
			decimal salary,
			string currency,
			DateTime? joiningDate,
			string? benefits,
			DateTime? expirationDate,
			string? createdBy)
		{
			return new Offers(
				id: 0,
				applicationId: applicationId,
				candidateId: candidateId,
				jobId: jobId,
				organizationId: organizationId,
				position: position,
				salary: salary,
				currency: currency,
				joiningDate: joiningDate,
				benefits: benefits,
				expirationDate: expirationDate,
				status: "Draft",
				sentDate: null,
				respondedDate: null,
				isActive: true,
				createdBy: createdBy,
				updatedOn: null,
				updatedBy: null);
		}

		public void Update(
			string position,
			decimal salary,
			string currency,
			DateTime? joiningDate,
			string? benefits,
			DateTime? expirationDate,
			string status,
			DateTime? sentDate,
			DateTime? respondedDate,
			bool isActive,
			string? updatedBy)
		{
			Position = position;
			Salary = salary;
			Currency = currency;
			JoiningDate = joiningDate;
			Benefits = benefits;
			ExpirationDate = expirationDate;
			Status = status;
			SentDate = sentDate;
			RespondedDate = respondedDate;
			IsActive = isActive;

			UpdatedBy = updatedBy;
			UpdatedOn = DateTime.Now;
		}
	}
}
