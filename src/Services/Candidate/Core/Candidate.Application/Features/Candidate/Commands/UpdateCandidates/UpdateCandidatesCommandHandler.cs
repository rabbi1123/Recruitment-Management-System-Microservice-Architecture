using Candidate.Application.Abstractions.Data;
using Candidate.Application.Common;
using Candidate.Domain.Candidate;
using Common.Platform.Domain.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candidate.Application.Features.Candidate.Commands.UpdateCandidates
{
	public class UpdateCandidatesCommandHandler : IRequestHandler<UpdateCandidatesCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Candidates> _repository;

		public UpdateCandidatesCommandHandler(IGenericRepository<Candidates> Repository)
        {
			_repository = Repository;
		}

        public async Task<Result<CommandResponse>> Handle(UpdateCandidatesCommand request, CancellationToken cancellationToken)
		{
			var conflictProp = new
			{
				UserId = request.UserId,
				Email = request.Email
			};

			var conflict = (await _repository.GetByPropertiesAsync(conflictProp, useOr: true))
						.FirstOrDefault(c => c.Id != request.Id);

			if (conflict is not null)
			{
				return Result.Failure<CommandResponse>(HttpResponseStatusCodes.Conflict, Error.Conflict("Candidates with this id or email"));
			}

			var candidate = await _repository.GetByIdAsync(request.Id);
			if (candidate is not null)
			{
				candidate.Update(
					request.FirstName,
					request.LastName,
					request.Email,
					request.Phone,
					request.Address,
					request.LinkedinProfile,
					request.PortfolioUrl,
					request.Headline,
					request.Summary,
					request.IsActive,
					request.UpdatedBy);

				var affectedRows = await _repository.UpdateAsync(candidate);

				if (affectedRows == 0)
				{
					return Result.Failure<CommandResponse>(
						HttpResponseStatusCodes.NotFound,
						Error.NotFound("Candidates"));
				}

				var response = new CommandResponse
				{
					IsSuccess = true
				};

				return response;
			}
			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("Candidate"));
		}
	}
}
