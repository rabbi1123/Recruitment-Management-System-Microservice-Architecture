using Job.Application.Abstractions.Data;
using Job.Application.Common;
using Job.Domain.Job;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Job.Application.Features.Job.Commands.AddJobs
{
	public class AddJobsCommandHandler : IRequestHandler<AddJobsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Jobs> _repository;

		public AddJobsCommandHandler(IGenericRepository<Jobs> repository)
		{
			_repository = repository;
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

			var affectedRows = await _repository.AddAsync(job);

			if (affectedRows == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("Jobs"));
			}

			return new CommandResponse { IsSuccess = true };
		}
	}
}
