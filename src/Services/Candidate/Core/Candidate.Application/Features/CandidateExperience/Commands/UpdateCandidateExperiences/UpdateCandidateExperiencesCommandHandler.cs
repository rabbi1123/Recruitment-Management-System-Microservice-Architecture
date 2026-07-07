using Candidate.Application.Abstractions.Data;
using Candidate.Application.Common;
using Candidate.Domain.CandidateExperience;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.CandidateExperience.Commands.UpdateCandidateExperiences
{
	public class UpdateCandidateExperiencesCommandHandler : IRequestHandler<UpdateCandidateExperiencesCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<CandidateExperiences> _repository;

		public UpdateCandidateExperiencesCommandHandler(IGenericRepository<CandidateExperiences> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(UpdateCandidateExperiencesCommand request, CancellationToken cancellationToken)
		{
			var experience = await _repository.GetByIdAsync(request.Id);
			if (experience is not null)
			{
				experience.Update(
					request.Company,
					request.Title,
					request.Location,
					request.StartDate,
					request.EndDate,
					request.IsCurrent,
					request.Description,
					request.IsActive,
					request.UpdatedBy);

				var affectedRows = await _repository.UpdateAsync(experience);

				if (affectedRows == 0)
				{
					return Result.Failure<CommandResponse>(
						HttpResponseStatusCodes.NotFound,
						Error.NotFound("CandidateExperiences"));
				}

				return new CommandResponse { IsSuccess = true };
			}

			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("CandidateExperience"));
		}
	}
}
