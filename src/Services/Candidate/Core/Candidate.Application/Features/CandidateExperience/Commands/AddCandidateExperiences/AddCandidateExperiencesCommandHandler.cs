using Candidate.Application.Abstractions.Data;
using Candidate.Application.Common;
using Candidate.Domain.CandidateExperience;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.CandidateExperience.Commands.AddCandidateExperiences
{
	public class AddCandidateExperiencesCommandHandler : IRequestHandler<AddCandidateExperiencesCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<CandidateExperiences> _repository;

		public AddCandidateExperiencesCommandHandler(IGenericRepository<CandidateExperiences> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(AddCandidateExperiencesCommand request, CancellationToken cancellationToken)
		{
			var experience = CandidateExperiences.Create(
				request.CandidateId,
				request.Company,
				request.Title,
				request.Location,
				request.StartDate,
				request.EndDate,
				request.IsCurrent,
				request.Description,
				request.CreatedBy);

			var affectedRows = await _repository.AddAsync(experience);

			if (affectedRows == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("CandidateExperiences"));
			}

			return new CommandResponse { IsSuccess = true };
		}
	}
}
