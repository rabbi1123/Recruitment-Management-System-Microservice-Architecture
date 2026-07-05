using Candidate.Application.Common;
using Candidate.Application.Common.CRUD.Commands;
using Candidate.Application.Common.CRUD.Queries;
using Candidate.Application.Features.Resume.Commands.AddResumes;
using Candidate.Application.Features.Resume.Commands.UpdateResumes;
using Candidate.Application.Features.Resume.Queries;
using Candidate.Domain.Resume;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Candidate.WebAPI.Controllers.Resume
{
	[Route(ApiRoutes.Candidate.Resumes)]
	public class ResumesController : ApiBaseController
	{
		public ResumesController(IMediator mediator) : base(mediator)
		{
		}

		[HttpPost(ApiRoutes.Common.GetAll)]
		[ProducesResponseType(typeof(Result<List<ResumesResponse>>), StatusCodes.Status200OK, "application/json")]
		[ProducesResponseType(typeof(Result<List<ResumesResponse>>), StatusCodes.Status400BadRequest, "application/json")]
		public async Task<IActionResult> GetAll(GetAllQuery<ResumesResponse> request)
		{
			var response = await QueryAsync(request);

			if (response.IsFailure)
			{
				return StatusCode(response.Status, response);
			}

			return Ok(response);
		}

		[HttpPost(ApiRoutes.Common.GetById)]
		[ProducesResponseType(typeof(Result<ResumesResponse>), StatusCodes.Status200OK, "application/json")]
		[ProducesResponseType(typeof(Result<ResumesResponse>), StatusCodes.Status400BadRequest, "application/json")]
		[ProducesResponseType(typeof(Result<ResumesResponse>), StatusCodes.Status404NotFound, "application/json")]
		public async Task<IActionResult> GetById([FromBody] GetByIdQuery<ResumesResponse> request)
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
		public async Task<IActionResult> Create([FromBody] AddResumesCommand request)
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
		public async Task<IActionResult> Update([FromBody] UpdateResumesCommand request)
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
		public async Task<IActionResult> Delete([FromBody] DeleteCommand<Resumes> request)
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
