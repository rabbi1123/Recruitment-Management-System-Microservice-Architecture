using Common.Platform.Domain.Abstractions;

namespace Recruitment.Domain.Application
{
	public class Applications : IEntity
	{
		public static readonly string[] AllowedStages =
		{
			"Applied",
			"UnderReview",
			"Shortlisted",
			"InterviewScheduled",
			"TechnicalInterview",
			"HRInterview",
			"OfferSent",
			"Hired",
			"Rejected"
		};

		public static readonly string[] AllowedStatuses =
		{
			"Active",
			"Rejected",
			"Withdrawn",
			"Hired"
		};

		public Applications(
			long id,
			long jobId,
			long candidateId,
			long organizationId,
			long? recruiterId,
			string currentStage,
			string status,
			string? coverLetter,
			string? rejectionReason,
			bool isActive,
			string? createdBy,
			DateTime? updatedOn,
			string? updatedBy)
		{
			Id = id;
			JobId = jobId;
			CandidateId = candidateId;
			OrganizationId = organizationId;
			RecruiterId = recruiterId;
			CurrentStage = currentStage;
			Status = status;
			CoverLetter = coverLetter;
			RejectionReason = rejectionReason;
			IsActive = isActive;

			CreatedBy = createdBy;
			UpdatedOn = updatedOn;
			UpdatedBy = updatedBy;
		}

		public Applications()
		{
		}

		public string GetPrimaryKeyName() => nameof(Id);

		public long GetPrimaryKeyValue() => Id;

		public long Id { get; set; }

		public long JobId { get; set; }

		public long CandidateId { get; set; }

		public long OrganizationId { get; set; }

		public long? RecruiterId { get; set; }

		public string CurrentStage { get; set; }

		public string Status { get; set; }

		public string? CoverLetter { get; set; }

		public string? RejectionReason { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedOn { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public string? UpdatedBy { get; set; }

		public static Applications Create(
			long jobId,
			long candidateId,
			long organizationId,
			long? recruiterId,
			string? coverLetter,
			string? createdBy)
		{
			return new Applications(
				id: 0,
				jobId: jobId,
				candidateId: candidateId,
				organizationId: organizationId,
				recruiterId: recruiterId,
				currentStage: "Applied",
				status: "Active",
				coverLetter: coverLetter,
				rejectionReason: null,
				isActive: true,
				createdBy: createdBy,
				updatedOn: null,
				updatedBy: null);
		}

		public void Update(
			long? recruiterId,
			string currentStage,
			string status,
			string? coverLetter,
			string? rejectionReason,
			bool isActive,
			string? updatedBy)
		{
			RecruiterId = recruiterId;
			CurrentStage = currentStage;
			Status = status;
			CoverLetter = coverLetter;
			RejectionReason = rejectionReason;
			IsActive = isActive;

			UpdatedBy = updatedBy;
			UpdatedOn = DateTime.Now;
		}
	}
}
