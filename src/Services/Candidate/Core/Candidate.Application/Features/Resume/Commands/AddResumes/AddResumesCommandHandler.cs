using Candidate.Application.Abstractions.Data;
using Candidate.Application.Common;
using Candidate.Domain.Resume;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.Resume.Commands.AddResumes
{
	public class AddResumesCommandHandler : IRequestHandler<AddResumesCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Resumes> _repository;

		public AddResumesCommandHandler(IGenericRepository<Resumes> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(AddResumesCommand request, CancellationToken cancellationToken)
		{
			var resume = Resumes.Create(
				request.CandidateId,
				request.FileName,
				request.FileUrl,
				request.FileFormat,
				request.FileSize,
				request.IsDefault,
				request.CreatedBy);

			var affectedRows = await _repository.AddAsync(resume);

			if (affectedRows == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("Resumes"));
			}

			return new CommandResponse { IsSuccess = true };
		}
	}
}
