using Candidate.Application.Abstractions.Data;
using Candidate.Application.Common;
using Candidate.Domain.CandidateCertification;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.CandidateCertification.Commands.UpdateCandidateCertifications
{
	public class UpdateCandidateCertificationsCommandHandler : IRequestHandler<UpdateCandidateCertificationsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<CandidateCertifications> _repository;

		public UpdateCandidateCertificationsCommandHandler(IGenericRepository<CandidateCertifications> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(UpdateCandidateCertificationsCommand request, CancellationToken cancellationToken)
		{
			var certification = await _repository.GetByIdAsync(request.Id);
			if (certification is not null)
			{
				certification.Update(
					request.Name,
					request.IssuingOrg,
					request.IssueDate,
					request.ExpiryDate,
					request.CredentialId,
					request.CredentialUrl,
					request.IsActive,
					request.UpdatedBy);

				var affectedRows = await _repository.UpdateAsync(certification);

				if (affectedRows == 0)
				{
					return Result.Failure<CommandResponse>(
						HttpResponseStatusCodes.NotFound,
						Error.NotFound("CandidateCertifications"));
				}

				return new CommandResponse { IsSuccess = true };
			}

			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("CandidateCertification"));
		}
	}
}
