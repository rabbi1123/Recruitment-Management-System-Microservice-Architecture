using Common.Platform.Domain.Abstractions;

namespace Candidate.Domain.CandidateCertification
{
	public class CandidateCertifications : IEntity
	{
		public CandidateCertifications(
			long id,
			long candidateId,
			string name,
			string? issuingOrg,
			DateOnly? issueDate,
			DateOnly? expiryDate,
			string? credentialId,
			string? credentialUrl,
			bool isActive,
			string? createdBy,
			DateTime? updatedOn,
			string? updatedBy)
		{
			Id = id;
			CandidateId = candidateId;
			Name = name;
			IssuingOrg = issuingOrg;
			IssueDate = issueDate;
			ExpiryDate = expiryDate;
			CredentialId = credentialId;
			CredentialUrl = credentialUrl;
			IsActive = isActive;

			CreatedBy = createdBy;
			UpdatedOn = updatedOn;
			UpdatedBy = updatedBy;
		}

		public CandidateCertifications()
		{
		}

		public string GetPrimaryKeyName() => nameof(Id);

		public long GetPrimaryKeyValue() => Id;

		public long Id { get; set; }

		public long CandidateId { get; set; }

		public string Name { get; set; }

		public string? IssuingOrg { get; set; }

		public DateOnly? IssueDate { get; set; }

		public DateOnly? ExpiryDate { get; set; }

		public string? CredentialId { get; set; }

		public string? CredentialUrl { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedOn { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public string? UpdatedBy { get; set; }

		public static CandidateCertifications Create(
			long candidateId,
			string name,
			string? issuingOrg,
			DateOnly? issueDate,
			DateOnly? expiryDate,
			string? credentialId,
			string? credentialUrl,
			string? createdBy)
		{
			return new CandidateCertifications(
				id: 0,
				candidateId: candidateId,
				name: name,
				issuingOrg: issuingOrg,
				issueDate: issueDate,
				expiryDate: expiryDate,
				credentialId: credentialId,
				credentialUrl: credentialUrl,
				isActive: true,
				createdBy: createdBy,
				updatedOn: null,
				updatedBy: null);
		}

		public void Update(
			string name,
			string? issuingOrg,
			DateOnly? issueDate,
			DateOnly? expiryDate,
			string? credentialId,
			string? credentialUrl,
			bool isActive,
			string? updatedBy)
		{
			Name = name;
			IssuingOrg = issuingOrg;
			IssueDate = issueDate;
			ExpiryDate = expiryDate;
			CredentialId = credentialId;
			CredentialUrl = credentialUrl;
			IsActive = isActive;

			UpdatedBy = updatedBy;
			UpdatedOn = DateTime.Now;
		}
	}
}
