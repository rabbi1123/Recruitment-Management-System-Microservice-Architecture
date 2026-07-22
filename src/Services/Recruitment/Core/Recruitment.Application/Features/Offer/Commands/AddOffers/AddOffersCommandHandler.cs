using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Common;
using Recruitment.Domain.Offer;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Recruitment.Application.Features.Offer.Commands.AddOffers
{
	public class AddOffersCommandHandler : IRequestHandler<AddOffersCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Offers> _repository;

		public AddOffersCommandHandler(IGenericRepository<Offers> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(AddOffersCommand request, CancellationToken cancellationToken)
		{
			var offer = Offers.Create(
				request.ApplicationId,
				request.CandidateId,
				request.JobId,
				request.OrganizationId,
				request.Position,
				request.Salary,
				request.Currency,
				request.JoiningDate,
				request.Benefits,
				request.ExpirationDate,
				request.CreatedBy);

			var affectedRows = await _repository.AddAsync(offer);

			if (affectedRows == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("Offers"));
			}

			return new CommandResponse { IsSuccess = true };
		}
	}
}
