using Job.Application.Abstractions.Data;
using Job.Application.Common;
using Job.Domain.SavedJob;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Job.Application.Features.SavedJob.Commands.AddSavedJobs
{
	public class AddSavedJobsCommandHandler : IRequestHandler<AddSavedJobsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<SavedJobs> _repository;

		public AddSavedJobsCommandHandler(IGenericRepository<SavedJobs> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(AddSavedJobsCommand request, CancellationToken cancellationToken)
		{
			var conflictProp = new
			{
				CandidateId = request.CandidateId,
				JobId = request.JobId,
			};

			var conflict = (await _repository.GetByPropertiesAsync(conflictProp)).FirstOrDefault();

			if (conflict is not null)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.Conflict,
					Error.Conflict("Saved job already exists for this candidate and job"));
			}

			var savedJob = SavedJobs.Create(
				request.CandidateId,
				request.JobId,
				request.CreatedBy);

			var affectedRows = await _repository.AddAsync(savedJob);

			if (affectedRows == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("SavedJobs"));
			}

			return new CommandResponse { IsSuccess = true };
		}
	}
}
