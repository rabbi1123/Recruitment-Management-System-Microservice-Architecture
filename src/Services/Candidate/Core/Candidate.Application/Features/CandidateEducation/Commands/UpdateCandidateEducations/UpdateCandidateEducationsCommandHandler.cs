using Candidate.Application.Abstractions.Data;
using Candidate.Application.Common;
using Candidate.Domain.CandidateEducation;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.CandidateEducation.Commands.UpdateCandidateEducations
{
	public class UpdateCandidateEducationsCommandHandler : IRequestHandler<UpdateCandidateEducationsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<CandidateEducations> _repository;

		public UpdateCandidateEducationsCommandHandler(IGenericRepository<CandidateEducations> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(UpdateCandidateEducationsCommand request, CancellationToken cancellationToken)
		{
			var education = await _repository.GetByIdAsync(request.Id);
			if (education is not null)
			{
				education.Update(
					request.Institution,
					request.Degree,
					request.FieldOfStudy,
					request.StartDate,
					request.EndDate,
					request.Grade,
					request.IsActive,
					request.UpdatedBy);

				var affectedRows = await _repository.UpdateAsync(education);

				if (affectedRows == 0)
				{
					return Result.Failure<CommandResponse>(
						HttpResponseStatusCodes.NotFound,
						Error.NotFound("CandidateEducations"));
				}

				return new CommandResponse { IsSuccess = true };
			}

			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("CandidateEducation"));
		}
	}
}
