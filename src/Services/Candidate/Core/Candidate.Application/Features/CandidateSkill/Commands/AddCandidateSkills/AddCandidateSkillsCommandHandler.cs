using Candidate.Application.Abstractions.Data;
using Candidate.Application.Common;
using Candidate.Domain.CandidateSkill;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.CandidateSkill.Commands.AddCandidateSkills
{
	public class AddCandidateSkillsCommandHandler : IRequestHandler<AddCandidateSkillsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<CandidateSkills> _repository;

		public AddCandidateSkillsCommandHandler(IGenericRepository<CandidateSkills> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(AddCandidateSkillsCommand request, CancellationToken cancellationToken)
		{
			var conflictProp = new
			{
				CandidateId = request.CandidateId,
				Name = request.Name,
			};

			var conflict = (await _repository.GetByPropertiesAsync(conflictProp)).FirstOrDefault();

			if (conflict is not null)
			{
				return Result.Failure<CommandResponse>(HttpResponseStatusCodes.Conflict, Error.Conflict("Candidate skill with this name already exists for the candidate"));
			}

			var skill = CandidateSkills.Create(
				request.CandidateId,
				request.Name,
				request.ProficiencyLevel,
				request.YearsOfExperience,
				request.CreatedBy);

			var affectedRows = await _repository.AddAsync(skill);

			if (affectedRows == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("CandidateSkills"));
			}

			return new CommandResponse { IsSuccess = true };
		}
	}
}
