using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Common;
using Recruitment.Domain.Application;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Recruitment.Application.Features.Application.Commands.UpdateApplications
{
	public class UpdateApplicationsCommandHandler : IRequestHandler<UpdateApplicationsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Applications> _repository;

		public UpdateApplicationsCommandHandler(IGenericRepository<Applications> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(UpdateApplicationsCommand request, CancellationToken cancellationToken)
		{
			var application = await _repository.GetByIdAsync(request.Id);
			if (application is not null)
			{
				application.Update(
					request.RecruiterId,
					request.CurrentStage,
					request.Status,
					request.CoverLetter,
					request.RejectionReason,
					request.IsActive,
					request.UpdatedBy);

				var affectedRows = await _repository.UpdateAsync(application);

				if (affectedRows == 0)
				{
					return Result.Failure<CommandResponse>(
						HttpResponseStatusCodes.NotFound,
						Error.NotFound("Applications"));
				}

				return new CommandResponse { IsSuccess = true };
			}

			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("Application"));
		}
	}
}
