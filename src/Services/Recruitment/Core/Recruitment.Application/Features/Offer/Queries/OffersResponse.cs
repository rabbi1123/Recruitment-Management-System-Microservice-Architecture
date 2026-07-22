using Recruitment.Application.Common.CRUD.Queries;

namespace Recruitment.Application.Features.Offer.Queries
{
	public class OffersResponse : IGenericResponse
	{
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
	}
}
