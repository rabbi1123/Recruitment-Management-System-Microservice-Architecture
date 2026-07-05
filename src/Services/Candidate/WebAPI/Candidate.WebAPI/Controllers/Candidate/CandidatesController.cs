using Candidate.Application.Common.CRUD.Commands;
using Candidate.Application.Common.CRUD.Queries;
using Candidate.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Candidate.Application.Features.Candidate.Commands.AddCandidates;
using Candidate.Application.Features.Candidate.Commands.UpdateCandidates;
using Candidate.Domain.Candidate;
using Candidate.Application.Features.Candidate.Queries;

namespace Candidate.WebAPI.Controllers.Candidate
{
	[Route(ApiRoutes.Candidate.Candidates)]
	public class CandidatesController : ApiBaseController
	{
        public CandidatesController(IMediator mediator) : base(mediator)
		{
        }

		[HttpPost(ApiRoutes.Common.GetAll)]
		[ProducesResponseType(typeof(Result<List<CandidatesResponse>>), StatusCodes.Status200OK, "application/json")]
		[ProducesResponseType(typeof(Result<List<CandidatesResponse>>), StatusCodes.Status400BadRequest, "application/json")]
		public async Task<IActionResult> GetAll(GetAllQuery<CandidatesResponse> request)
		{
			var response = await QueryAsync(request);

			if (response.IsFailure)
			{
				return StatusCode(response.Status, response);
			}

			return Ok(response);
		}

		[HttpPost(ApiRoutes.Common.GetById)]
		[ProducesResponseType(typeof(Result<CandidatesResponse>), StatusCodes.Status200OK, "application/json")]
		[ProducesResponseType(typeof(Result<CandidatesResponse>), StatusCodes.Status400BadRequest, "application/json")]
		[ProducesResponseType(typeof(Result<CandidatesResponse>), StatusCodes.Status404NotFound, "application/json")]
		public async Task<IActionResult> GetById([FromBody] GetByIdQuery<CandidatesResponse> request)
		{
			var response = await QueryAsync(request);

			if (response.IsFailure)
			{
				return StatusCode(response.Status, response);
			}

			return Ok(response);
		}

		[HttpPost(ApiRoutes.Common.Create)]
		[ProducesResponseType(typeof(Result<CommandResponse>), StatusCodes.Status200OK, "application/json")]
		[ProducesResponseType(typeof(Result<CommandResponse>), StatusCodes.Status400BadRequest, "application/json")]
		public async Task<IActionResult> Create([FromBody] AddCandidatesCommand request)
		{
			var response = await CommandAsync(request);

			if (response.IsFailure)
			{
				return StatusCode(response.Status, response);
			}

			return Ok(response);
		}

		[HttpPost(ApiRoutes.Common.Update)]
		[ProducesResponseType(typeof(Result<CommandResponse>), StatusCodes.Status200OK, "application/json")]
		[ProducesResponseType(typeof(Result<CommandResponse>), StatusCodes.Status400BadRequest, "application/json")]
		[ProducesResponseType(typeof(Result<CommandResponse>), StatusCodes.Status404NotFound, "application/json")]
		public async Task<IActionResult> Update([FromBody] UpdateCandidatesCommand request)
		{
			var response = await CommandAsync(request);

			if (response.IsFailure)
			{
				return StatusCode(response.Status, response);
			}

			return Ok(response);
		}

		[HttpPost(ApiRoutes.Common.Delete)]
		[ProducesResponseType(typeof(Result<CommandResponse>), StatusCodes.Status200OK, "application/json")]
		[ProducesResponseType(typeof(Result<CommandResponse>), StatusCodes.Status400BadRequest, "application/json")]
		[ProducesResponseType(typeof(Result<CommandResponse>), StatusCodes.Status404NotFound, "application/json")]
		public async Task<IActionResult> Delete([FromBody] DeleteCommand<Candidates> request)
		{
			var response = await CommandAsync(request);

			if (response.IsFailure)
			{
				return StatusCode(response.Status, response);
			}

			return Ok(response);
		}
	}
}
