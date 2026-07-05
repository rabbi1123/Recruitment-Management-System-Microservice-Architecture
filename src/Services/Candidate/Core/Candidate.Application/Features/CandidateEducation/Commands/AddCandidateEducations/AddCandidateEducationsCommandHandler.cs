using Candidate.Application.Abstractions.Data;
using Candidate.Application.Common;
using Candidate.Domain.CandidateEducation;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.CandidateEducation.Commands.AddCandidateEducations
{
	public class AddCandidateEducationsCommandHandler : IRequestHandler<AddCandidateEducationsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<CandidateEducations> _repository;

		public AddCandidateEducationsCommandHandler(IGenericRepository<CandidateEducations> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(AddCandidateEducationsCommand request, CancellationToken cancellationToken)
		{
			var education = CandidateEducations.Create(
				request.CandidateId,
				request.Institution,
				request.Degree,
				request.FieldOfStudy,
				request.StartDate,
				request.EndDate,
				request.Grade,
				request.CreatedBy);

			var affectedRows = await _repository.AddAsync(education);

			if (affectedRows == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("CandidateEducations"));
			}

			return new CommandResponse { IsSuccess = true };
		}
	}
}
