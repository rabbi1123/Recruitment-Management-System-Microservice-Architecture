using Job.Application.Abstractions.Data;
using Job.Application.Common;
using Job.Domain.SavedJob;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Job.Application.Features.SavedJob.Commands.UpdateSavedJobs
{
	public class UpdateSavedJobsCommandHandler : IRequestHandler<UpdateSavedJobsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<SavedJobs> _repository;

		public UpdateSavedJobsCommandHandler(IGenericRepository<SavedJobs> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(UpdateSavedJobsCommand request, CancellationToken cancellationToken)
		{
			var conflictProp = new
			{
				CandidateId = request.CandidateId,
				JobId = request.JobId,
			};

			var conflict = (await _repository.GetByPropertiesAsync(conflictProp))
				.FirstOrDefault(s => s.Id != request.Id);

			if (conflict is not null)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.Conflict,
					Error.Conflict("Saved job already exists for this candidate and job"));
			}

			var savedJob = await _repository.GetByIdAsync(request.Id);
			if (savedJob is not null)
			{
				savedJob.Update(
					request.IsActive,
					request.UpdatedBy);

				var affectedRows = await _repository.UpdateAsync(savedJob);

				if (affectedRows == 0)
				{
					return Result.Failure<CommandResponse>(
						HttpResponseStatusCodes.NotFound,
						Error.NotFound("SavedJobs"));
				}

				return new CommandResponse { IsSuccess = true };
			}

			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("SavedJob"));
		}
	}
}
