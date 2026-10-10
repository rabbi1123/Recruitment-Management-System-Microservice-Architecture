using Job.Application.Abstractions.Data;
using Job.Application.Common;
using Job.Domain.Job;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Messaging.Contracts.Events.Email;
using Messaging.Contracts.Events.Push;
using Messaging.Contracts.Events.Sms;
using Messaging.RabbitMq;

namespace Job.Application.Features.Job.Commands.AddJobs
{
	public class AddJobsCommandHandler : IRequestHandler<AddJobsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Jobs> _repository;
		private readonly IPostCommitActions _postCommit;
		private readonly IIntegrationEventPublisher _publisher;

		public AddJobsCommandHandler(
			IGenericRepository<Jobs> repository,
			IPostCommitActions postCommit,
			IIntegrationEventPublisher publisher)
		{
			_repository = repository;
			_postCommit = postCommit;
			_publisher = publisher;
		}

		public async Task<Result<CommandResponse>> Handle(AddJobsCommand request, CancellationToken cancellationToken)
		{
			var job = Jobs.Create(
				request.OrganizationId,
				request.RecruiterId,
				request.Title,
				request.Department,
				request.EmploymentType,
				request.Location,
				request.IsRemote,
				request.ExperienceMin,
				request.ExperienceMax,
				request.SalaryMin,
				request.SalaryMax,
				request.Currency,
				request.Description,
				request.Status,
				request.CreatedBy);

			var id = await _repository.AddAsync(job);

			if (id == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("Jobs"));
			}

			if (job.Status == "Published")
			{
				_postCommit.Add(async ct =>
				{
					var publishedOnUtc = DateTime.UtcNow;
					await _publisher.PublishAsync(
						new JobPublishedEmailEvent(id, job.OrganizationId, job.Title, publishedOnUtc), ct);
					await _publisher.PublishAsync(
						new JobPublishedSmsEvent(id, job.OrganizationId, job.Title, publishedOnUtc), ct);
					await _publisher.PublishAsync(
						new JobPublishedPushEvent(id, job.OrganizationId, job.Title, publishedOnUtc), ct);
				});
			}

			return new CommandResponse { IsSuccess = true, Id = id };
		}
	}
}
