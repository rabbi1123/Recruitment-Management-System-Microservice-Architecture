using Job.Application.Abstractions.Data;
using Job.Application.Common;
using Job.Domain.Job;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Messaging.Contracts.Events.Email;
using Messaging.Contracts.Events.Push;
using Messaging.Contracts.Events.Sms;
using Messaging.RabbitMq;

namespace Job.Application.Features.Job.Commands.UpdateJobs
{
	public class UpdateJobsCommandHandler : IRequestHandler<UpdateJobsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Jobs> _repository;
		private readonly IPostCommitActions _postCommit;
		private readonly IIntegrationEventPublisher _publisher;

		public UpdateJobsCommandHandler(
			IGenericRepository<Jobs> repository,
			IPostCommitActions postCommit,
			IIntegrationEventPublisher publisher)
		{
			_repository = repository;
			_postCommit = postCommit;
			_publisher = publisher;
		}

		public async Task<Result<CommandResponse>> Handle(UpdateJobsCommand request, CancellationToken cancellationToken)
		{
			var job = await _repository.GetByIdAsync(request.Id);
			if (job is not null)
			{
				var previousStatus = job.Status;

				job.Update(
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
					request.IsActive,
					request.UpdatedBy);

				var affectedRows = await _repository.UpdateAsync(job);

				if (affectedRows == 0)
				{
					return Result.Failure<CommandResponse>(
						HttpResponseStatusCodes.NotFound,
						Error.NotFound("Jobs"));
				}

				if (previousStatus != "Published" && job.Status == "Published")
				{
					_postCommit.Add(async ct =>
					{
						var publishedOnUtc = DateTime.UtcNow;
						await _publisher.PublishAsync(
							new JobPublishedEmailEvent(job.Id, job.OrganizationId, job.Title, publishedOnUtc), ct);
						await _publisher.PublishAsync(
							new JobPublishedSmsEvent(job.Id, job.OrganizationId, job.Title, publishedOnUtc), ct);
						await _publisher.PublishAsync(
							new JobPublishedPushEvent(job.Id, job.OrganizationId, job.Title, publishedOnUtc), ct);
					});
				}

				return new CommandResponse { IsSuccess = true };
			}

			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("Job"));
		}
	}
}
