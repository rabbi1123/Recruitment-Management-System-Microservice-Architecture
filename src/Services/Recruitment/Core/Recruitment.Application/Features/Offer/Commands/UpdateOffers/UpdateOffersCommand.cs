using Recruitment.Application.Abstractions.CRUD;
using Recruitment.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Recruitment.Application.Features.Offer.Commands.UpdateOffers
{
	public record UpdateOffersCommand : IRequest<Result<CommandResponse>>, IUpdatedByCommand
	{
		public long Id { get; set; }
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
		public string? UpdatedBy { get; set; }
	}
}
