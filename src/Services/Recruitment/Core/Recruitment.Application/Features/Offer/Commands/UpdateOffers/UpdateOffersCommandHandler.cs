using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Common;
using Recruitment.Domain.Offer;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Recruitment.Application.Features.Offer.Commands.UpdateOffers
{
	public class UpdateOffersCommandHandler : IRequestHandler<UpdateOffersCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Offers> _repository;

		public UpdateOffersCommandHandler(IGenericRepository<Offers> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(UpdateOffersCommand request, CancellationToken cancellationToken)
		{
			var offer = await _repository.GetByIdAsync(request.Id);
			if (offer is not null)
			{
				offer.Update(
					request.Position,
					request.Salary,
					request.Currency,
					request.JoiningDate,
					request.Benefits,
					request.ExpirationDate,
					request.Status,
					request.SentDate,
					request.RespondedDate,
					request.IsActive,
					request.UpdatedBy);

				var affectedRows = await _repository.UpdateAsync(offer);

				if (affectedRows == 0)
				{
					return Result.Failure<CommandResponse>(
						HttpResponseStatusCodes.NotFound,
						Error.NotFound("Offers"));
				}

				return new CommandResponse { IsSuccess = true };
			}

			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("Offer"));
		}
	}
}
