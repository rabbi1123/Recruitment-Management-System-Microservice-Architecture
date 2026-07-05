using Candidate.Application.Common;
using Candidate.Application.Common.CRUD.Commands;
using Candidate.Application.Common.CRUD.Queries;
using Candidate.Application.Features.CandidateExperience.Commands.AddCandidateExperiences;
using Candidate.Application.Features.CandidateExperience.Commands.UpdateCandidateExperiences;
using Candidate.Application.Features.CandidateExperience.Queries;
using Candidate.Domain.CandidateExperience;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Candidate.WebAPI.Controllers.CandidateExperience
{
	[Route(ApiRoutes.Candidate.CandidateExperiences)]
	public class CandidateExperiencesController : ApiBaseController
	{
		public CandidateExperiencesController(IMediator mediator) : base(mediator)
		{
		}

		[HttpPost(ApiRoutes.Common.GetAll)]
		[ProducesResponseType(typeof(Result<List<CandidateExperiencesResponse>>), StatusCodes.Status200OK, "application/json")]
		[ProducesResponseType(typeof(Result<List<CandidateExperiencesResponse>>), StatusCodes.Status400BadRequest, "application/json")]
		public async Task<IActionResult> GetAll(GetAllQuery<CandidateExperiencesResponse> request)
		{
			var response = await QueryAsync(request);

			if (response.IsFailure)
			{
				return StatusCode(response.Status, response);
			}

			return Ok(response);
		}

		[HttpPost(ApiRoutes.Common.GetById)]
		[ProducesResponseType(typeof(Result<CandidateExperiencesResponse>), StatusCodes.Status200OK, "application/json")]
		[ProducesResponseType(typeof(Result<CandidateExperiencesResponse>), StatusCodes.Status400BadRequest, "application/json")]
		[ProducesResponseType(typeof(Result<CandidateExperiencesResponse>), StatusCodes.Status404NotFound, "application/json")]
		public async Task<IActionResult> GetById([FromBody] GetByIdQuery<CandidateExperiencesResponse> request)
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
		public async Task<IActionResult> Create([FromBody] AddCandidateExperiencesCommand request)
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
		public async Task<IActionResult> Update([FromBody] UpdateCandidateExperiencesCommand request)
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
		public async Task<IActionResult> Delete([FromBody] DeleteCommand<CandidateExperiences> request)
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
