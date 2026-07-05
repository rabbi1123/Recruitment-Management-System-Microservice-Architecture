using Candidate.Application.Abstractions.Data;
using Candidate.Application.Common;
using Candidate.Domain.CandidateSkill;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.CandidateSkill.Commands.UpdateCandidateSkills
{
	public class UpdateCandidateSkillsCommandHandler : IRequestHandler<UpdateCandidateSkillsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<CandidateSkills> _repository;

		public UpdateCandidateSkillsCommandHandler(IGenericRepository<CandidateSkills> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(UpdateCandidateSkillsCommand request, CancellationToken cancellationToken)
		{
			var conflictProp = new
			{
				CandidateId = request.CandidateId,
				Name = request.Name,
			};

			var conflict = (await _repository.GetByPropertiesAsync(conflictProp))
				.FirstOrDefault(s => s.Id != request.Id);

			if (conflict is not null)
			{
				return Result.Failure<CommandResponse>(HttpResponseStatusCodes.Conflict, Error.Conflict("Candidate skill with this name already exists for the candidate"));
			}

			var skill = await _repository.GetByIdAsync(request.Id);
			if (skill is not null)
			{
				skill.Update(
					request.Name,
					request.ProficiencyLevel,
					request.YearsOfExperience,
					request.IsActive,
					request.UpdatedBy);

				var affectedRows = await _repository.UpdateAsync(skill);

				if (affectedRows == 0)
				{
					return Result.Failure<CommandResponse>(
						HttpResponseStatusCodes.NotFound,
						Error.NotFound("CandidateSkills"));
				}

				return new CommandResponse { IsSuccess = true };
			}

			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("CandidateSkill"));
		}
	}
}
