using Job.Application.Abstractions.Data;
using Job.Application.Common;
using Job.Domain.Job;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Job.Application.Features.Job.Commands.UpdateJobs
{
	public class UpdateJobsCommandHandler : IRequestHandler<UpdateJobsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Jobs> _repository;

		public UpdateJobsCommandHandler(IGenericRepository<Jobs> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(UpdateJobsCommand request, CancellationToken cancellationToken)
		{
			var job = await _repository.GetByIdAsync(request.Id);
			if (job is not null)
			{
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

				return new CommandResponse { IsSuccess = true };
			}

			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("Job"));
		}
	}
}
