using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Common;
using Recruitment.Domain.Offer;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Messaging.Contracts.Events.Email;
using Messaging.Contracts.Events.Push;
using Messaging.Contracts.Events.Sms;
using Messaging.RabbitMq;

namespace Recruitment.Application.Features.Offer.Commands.UpdateOffers
{
	public class UpdateOffersCommandHandler : IRequestHandler<UpdateOffersCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Offers> _repository;
		private readonly IPostCommitActions _postCommit;
		private readonly IIntegrationEventPublisher _publisher;

		public UpdateOffersCommandHandler(
			IGenericRepository<Offers> repository,
			IPostCommitActions postCommit,
			IIntegrationEventPublisher publisher)
		{
			_repository = repository;
			_postCommit = postCommit;
			_publisher = publisher;
		}

		public async Task<Result<CommandResponse>> Handle(UpdateOffersCommand request, CancellationToken cancellationToken)
		{
			var offer = await _repository.GetByIdAsync(request.Id);
			if (offer is not null)
			{
				var previousStatus = offer.Status;

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

				if (previousStatus != "Sent" && offer.Status == "Sent")
				{
					_postCommit.Add(async ct =>
					{
						var sentOnUtc = DateTime.UtcNow;
						await _publisher.PublishAsync(
							new OfferSentEmailEvent(
								offer.Id,
								offer.ApplicationId,
								offer.CandidateId,
								offer.JobId,
								offer.OrganizationId,
								sentOnUtc),
							ct);
						await _publisher.PublishAsync(
							new OfferSentSmsEvent(
								offer.Id,
								offer.ApplicationId,
								offer.CandidateId,
								offer.JobId,
								offer.OrganizationId,
								sentOnUtc),
							ct);
						await _publisher.PublishAsync(
							new OfferSentPushEvent(
								offer.Id,
								offer.ApplicationId,
								offer.CandidateId,
								offer.JobId,
								offer.OrganizationId,
								sentOnUtc),
							ct);
					});
				}

				return new CommandResponse { IsSuccess = true };
			}

			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("Offer"));
		}
	}
}
