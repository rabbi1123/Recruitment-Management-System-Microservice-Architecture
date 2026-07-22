using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Common;
using Recruitment.Domain.Application;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Recruitment.Application.Features.Application.Commands.AddApplications
{
	public class AddApplicationsCommandHandler : IRequestHandler<AddApplicationsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Applications> _repository;

		public AddApplicationsCommandHandler(IGenericRepository<Applications> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(AddApplicationsCommand request, CancellationToken cancellationToken)
		{
			var conflictProp = new
			{
				request.JobId,
				request.CandidateId,
			};

			var conflict = (await _repository.GetByPropertiesAsync(conflictProp)).FirstOrDefault();

			if (conflict is not null)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.Conflict,
					Error.Conflict("Application already exists for this job and candidate"));
			}

			var application = Applications.Create(
				request.JobId,
				request.CandidateId,
				request.OrganizationId,
				request.RecruiterId,
				request.CoverLetter,
				request.CreatedBy);

			var affectedRows = await _repository.AddAsync(application);

			if (affectedRows == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("Applications"));
			}

			return new CommandResponse { IsSuccess = true };
		}
	}
}
