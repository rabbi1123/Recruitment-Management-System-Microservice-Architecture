using Candidate.Application.Common;
using Candidate.Application.Common.CRUD.Commands;
using Candidate.Application.Common.CRUD.Queries;
using Candidate.Application.Features.CandidateSkill.Commands.AddCandidateSkills;
using Candidate.Application.Features.CandidateSkill.Commands.UpdateCandidateSkills;
using Candidate.Application.Features.CandidateSkill.Queries;
using Candidate.Domain.CandidateSkill;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Candidate.WebAPI.Controllers.CandidateSkill
{
	[Route(ApiRoutes.Candidate.CandidateSkills)]
	public class CandidateSkillsController : ApiBaseController
	{
		public CandidateSkillsController(IMediator mediator) : base(mediator)
		{
		}

		[HttpPost(ApiRoutes.Common.GetAll)]
		[ProducesResponseType(typeof(Result<List<CandidateSkillsResponse>>), StatusCodes.Status200OK, "application/json")]
		[ProducesResponseType(typeof(Result<List<CandidateSkillsResponse>>), StatusCodes.Status400BadRequest, "application/json")]
		public async Task<IActionResult> GetAll(GetAllQuery<CandidateSkillsResponse> request)
		{
			var response = await QueryAsync(request);

			if (response.IsFailure)
			{
				return StatusCode(response.Status, response);
			}

			return Ok(response);
		}

		[HttpPost(ApiRoutes.Common.GetById)]
		[ProducesResponseType(typeof(Result<CandidateSkillsResponse>), StatusCodes.Status200OK, "application/json")]
		[ProducesResponseType(typeof(Result<CandidateSkillsResponse>), StatusCodes.Status400BadRequest, "application/json")]
		[ProducesResponseType(typeof(Result<CandidateSkillsResponse>), StatusCodes.Status404NotFound, "application/json")]
		public async Task<IActionResult> GetById([FromBody] GetByIdQuery<CandidateSkillsResponse> request)
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
		public async Task<IActionResult> Create([FromBody] AddCandidateSkillsCommand request)
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
		public async Task<IActionResult> Update([FromBody] UpdateCandidateSkillsCommand request)
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
		public async Task<IActionResult> Delete([FromBody] DeleteCommand<CandidateSkills> request)
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
