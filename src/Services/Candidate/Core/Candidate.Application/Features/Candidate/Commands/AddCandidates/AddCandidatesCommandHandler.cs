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
using static System.Net.Mime.MediaTypeNames;

namespace Candidate.Application.Features.Candidate.Commands.AddCandidates
{
	public class AddCandidatesCommandHandler : IRequestHandler<AddCandidatesCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Candidates> _repository;

		public AddCandidatesCommandHandler(IGenericRepository<Candidates> Repository)
        {
			_repository = Repository;
		}
        public async Task<Result<CommandResponse>> Handle(AddCandidatesCommand request, CancellationToken cancellationToken)
		{
			var conflictProp = new
			{
				UserId = request.UserId,
				Email = request.Email,
			};

			var conflict = (await _repository.GetByPropertiesAsync(conflictProp, useOr: true)).FirstOrDefault();

			if (conflict is not null)
			{
				return Result.Failure<CommandResponse>(HttpResponseStatusCodes.Conflict, Error.Conflict("Candidates with this user id or email"));
			}

			var candidate = Candidates.Create(
					request.UserId,
					request.FirstName,
					request.LastName,
					request.Email,
					request.Phone,
					request.Address,
					request.LinkedinProfile,
					request.PortfolioUrl,
					request.Headline,
					request.Summary,
					request.CreatedBy);

			var affectedRows = await _repository.AddAsync(candidate);

			if (affectedRows == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("Candidates"));
			}

			var response = new CommandResponse
			{
				IsSuccess = true
			};

			return response;
		}
	}
}
