using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Common;
using Recruitment.Domain.Application;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Messaging.Contracts.Events.Email;
using Messaging.Contracts.Events.Push;
using Messaging.Contracts.Events.Sms;
using Messaging.RabbitMq;

namespace Recruitment.Application.Features.Application.Commands.AddApplications
{
	public class AddApplicationsCommandHandler : IRequestHandler<AddApplicationsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Applications> _repository;
		private readonly IPostCommitActions _postCommit;
		private readonly IIntegrationEventPublisher _publisher;

		public AddApplicationsCommandHandler(
			IGenericRepository<Applications> repository,
			IPostCommitActions postCommit,
			IIntegrationEventPublisher publisher)
		{
			_repository = repository;
			_postCommit = postCommit;
			_publisher = publisher;
		}

		public async Task<Result<CommandResponse>> Handle(AddApplicationsCommand request, CancellationToken cancellationToken)
		{
			var conflictProp = new
			{
				request.JobId,
				request.CandidateId,
			};

			var conflict = (await _repository.GetByPropertiesAsync(conflictProp)).FirstOrDefault();

			if (conflict is not null)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.Conflict,
					Error.Conflict("Application already exists for this job and candidate"));
			}

			var application = Applications.Create(
				request.JobId,
				request.CandidateId,
				request.OrganizationId,
				request.RecruiterId,
				request.CoverLetter,
				request.CreatedBy);

			var id = await _repository.AddAsync(application);

			if (id == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("Applications"));
			}

			_postCommit.Add(async ct =>
			{
				var occurredOnUtc = DateTime.UtcNow;
				await _publisher.PublishAsync(
					new ApplicationSubmittedEmailEvent(id, application.JobId, application.CandidateId, application.OrganizationId, occurredOnUtc),
					ct);
				await _publisher.PublishAsync(
					new ApplicationSubmittedSmsEvent(id, application.JobId, application.CandidateId, application.OrganizationId, occurredOnUtc),
					ct);
				await _publisher.PublishAsync(
					new ApplicationSubmittedPushEvent(id, application.JobId, application.CandidateId, application.OrganizationId, occurredOnUtc),
					ct);
			});

			return new CommandResponse { IsSuccess = true, Id = id };
		}
	}
}
