using Recruitment.Application.Abstractions.CRUD;
using Recruitment.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Recruitment.Application.Features.Offer.Commands.AddOffers
{
	public record AddOffersCommand : IRequest<Result<CommandResponse>>, ICreatedByCommand
	{
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
		public string? CreatedBy { get; set; }
	}
}
