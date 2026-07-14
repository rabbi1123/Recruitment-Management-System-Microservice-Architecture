using Job.Application.Abstractions.Data;
using Job.Application.Common;
using Job.Domain.JobSkill;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Job.Application.Features.JobSkill.Commands.UpdateJobSkills
{
	public class UpdateJobSkillsCommandHandler : IRequestHandler<UpdateJobSkillsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<JobSkills> _repository;

		public UpdateJobSkillsCommandHandler(IGenericRepository<JobSkills> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(UpdateJobSkillsCommand request, CancellationToken cancellationToken)
		{
			var conflictProp = new
			{
				JobId = request.JobId,
				SkillName = request.SkillName,
			};

			var conflict = (await _repository.GetByPropertiesAsync(conflictProp))
				.FirstOrDefault(s => s.Id != request.Id);

			if (conflict is not null)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.Conflict,
					Error.Conflict("Job skill with this name already exists for the job"));
			}

			var skill = await _repository.GetByIdAsync(request.Id);
			if (skill is not null)
			{
				skill.Update(
					request.SkillName,
					request.IsRequired,
					request.IsActive,
					request.UpdatedBy);

				var affectedRows = await _repository.UpdateAsync(skill);

				if (affectedRows == 0)
				{
					return Result.Failure<CommandResponse>(
						HttpResponseStatusCodes.NotFound,
						Error.NotFound("JobSkills"));
				}

				return new CommandResponse { IsSuccess = true };
			}

			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("JobSkill"));
		}
	}
}
