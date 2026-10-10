using Organization.Application.Abstractions.Data;
using Organization.Application.Common;
using Organization.Domain.Organization;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Messaging.Contracts.Events.Email;
using Messaging.Contracts.Events.Push;
using Messaging.Contracts.Events.Sms;
using Messaging.RabbitMq;

namespace Organization.Application.Features.Organization.Commands.AddOrganizations
{
	public class AddOrganizationsCommandHandler : IRequestHandler<AddOrganizationsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Organizations> _repository;
		private readonly IPostCommitActions _postCommit;
		private readonly IIntegrationEventPublisher _publisher;

		public AddOrganizationsCommandHandler(
			IGenericRepository<Organizations> repository,
			IPostCommitActions postCommit,
			IIntegrationEventPublisher publisher)
		{
			_repository = repository;
			_postCommit = postCommit;
			_publisher = publisher;
		}

		public async Task<Result<CommandResponse>> Handle(AddOrganizationsCommand request, CancellationToken cancellationToken)
		{
			var organization = Organizations.Create(
				request.Name,
				request.Industry,
				request.Website,
				request.Email,
				request.Phone,
				request.Address,
				request.LogoUrl,
				request.CreatedBy);

			var id = await _repository.AddAsync(organization);

			if (id == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("Organizations"));
			}

			_postCommit.Add(async ct =>
			{
				var occurredOnUtc = DateTime.UtcNow;
				await _publisher.PublishAsync(
					new OrganizationCreatedEmailEvent(id, organization.Name, occurredOnUtc), ct);
				await _publisher.PublishAsync(
					new OrganizationCreatedSmsEvent(id, organization.Name, occurredOnUtc), ct);
				await _publisher.PublishAsync(
					new OrganizationCreatedPushEvent(id, organization.Name, occurredOnUtc), ct);
			});

			return new CommandResponse { IsSuccess = true, Id = id };
		}
	}
}
