using Candidate.Application.Abstractions.Data;
using Candidate.Application.Common;
using Candidate.Domain.CandidateCertification;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.CandidateCertification.Commands.AddCandidateCertifications
{
	public class AddCandidateCertificationsCommandHandler : IRequestHandler<AddCandidateCertificationsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<CandidateCertifications> _repository;

		public AddCandidateCertificationsCommandHandler(IGenericRepository<CandidateCertifications> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(AddCandidateCertificationsCommand request, CancellationToken cancellationToken)
		{
			var certification = CandidateCertifications.Create(
				request.CandidateId,
				request.Name,
				request.IssuingOrg,
				request.IssueDate,
				request.ExpiryDate,
				request.CredentialId,
				request.CredentialUrl,
				request.CreatedBy);

			var affectedRows = await _repository.AddAsync(certification);

			if (affectedRows == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("CandidateCertifications"));
			}

			return new CommandResponse { IsSuccess = true };
		}
	}
}
