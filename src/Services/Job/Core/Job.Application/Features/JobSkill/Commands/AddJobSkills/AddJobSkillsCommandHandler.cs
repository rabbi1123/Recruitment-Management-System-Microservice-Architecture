using Job.Application.Abstractions.Data;
using Job.Application.Common;
using Job.Domain.JobSkill;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Job.Application.Features.JobSkill.Commands.AddJobSkills
{
	public class AddJobSkillsCommandHandler : IRequestHandler<AddJobSkillsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<JobSkills> _repository;

		public AddJobSkillsCommandHandler(IGenericRepository<JobSkills> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(AddJobSkillsCommand request, CancellationToken cancellationToken)
		{
			var conflictProp = new
			{
				JobId = request.JobId,
				SkillName = request.SkillName,
			};

			var conflict = (await _repository.GetByPropertiesAsync(conflictProp)).FirstOrDefault();

			if (conflict is not null)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.Conflict,
					Error.Conflict("Job skill with this name already exists for the job"));
			}

			var skill = JobSkills.Create(
				request.JobId,
				request.SkillName,
				request.IsRequired,
				request.CreatedBy);

			var affectedRows = await _repository.AddAsync(skill);

			if (affectedRows == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("JobSkills"));
			}

			return new CommandResponse { IsSuccess = true };
		}
	}
}
