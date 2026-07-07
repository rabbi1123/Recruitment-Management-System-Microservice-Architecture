using Candidate.Application.Abstractions.Data;
using Candidate.Application.Common;
using Candidate.Domain.Resume;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.Resume.Commands.UpdateResumes
{
	public class UpdateResumesCommandHandler : IRequestHandler<UpdateResumesCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Resumes> _repository;

		public UpdateResumesCommandHandler(IGenericRepository<Resumes> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(UpdateResumesCommand request, CancellationToken cancellationToken)
		{
			var resume = await _repository.GetByIdAsync(request.Id);
			if (resume is not null)
			{
				resume.Update(
					request.FileName,
					request.FileUrl,
					request.FileFormat,
					request.FileSize,
					request.IsDefault,
					request.IsActive,
					request.UpdatedBy);

				var affectedRows = await _repository.UpdateAsync(resume);

				if (affectedRows == 0)
				{
					return Result.Failure<CommandResponse>(
						HttpResponseStatusCodes.NotFound,
						Error.NotFound("Resumes"));
				}

				return new CommandResponse { IsSuccess = true };
			}

			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("Resume"));
		}
	}
}
