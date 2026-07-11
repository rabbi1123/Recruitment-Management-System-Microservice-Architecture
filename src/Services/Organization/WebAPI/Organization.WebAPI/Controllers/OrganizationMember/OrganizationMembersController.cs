using Organization.Application.Common;
using Organization.Application.Common.CRUD.Commands;
using Organization.Application.Common.CRUD.Queries;
using Organization.Application.Features.OrganizationMember.Commands.AddOrganizationMembers;
using Organization.Application.Features.OrganizationMember.Commands.UpdateOrganizationMembers;
using Organization.Application.Features.OrganizationMember.Queries;
using Organization.Domain.OrganizationMember;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Organization.WebAPI.Controllers.OrganizationMember
{
	[Route(ApiRoutes.Organization.OrganizationMembers)]
	public class OrganizationMembersController : ApiBaseController
	{
		public OrganizationMembersController(IMediator mediator) : base(mediator)
		{
		}

		[HttpPost(ApiRoutes.Common.GetAll)]
		[ProducesResponseType(typeof(Result<List<OrganizationMembersResponse>>), StatusCodes.Status200OK, "application/json")]
		[ProducesResponseType(typeof(Result<List<OrganizationMembersResponse>>), StatusCodes.Status400BadRequest, "application/json")]
		public async Task<IActionResult> GetAll(GetAllQuery<OrganizationMembersResponse> request)
		{
			var response = await QueryAsync(request);

			if (response.IsFailure)
			{
				return StatusCode(response.Status, response);
			}

			return Ok(response);
		}

		[HttpPost(ApiRoutes.Common.GetById)]
		[ProducesResponseType(typeof(Result<OrganizationMembersResponse>), StatusCodes.Status200OK, "application/json")]
		[ProducesResponseType(typeof(Result<OrganizationMembersResponse>), StatusCodes.Status400BadRequest, "application/json")]
		[ProducesResponseType(typeof(Result<OrganizationMembersResponse>), StatusCodes.Status404NotFound, "application/json")]
		public async Task<IActionResult> GetById([FromBody] GetByIdQuery<OrganizationMembersResponse> request)
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
		public async Task<IActionResult> Create([FromBody] AddOrganizationMembersCommand request)
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
		public async Task<IActionResult> Update([FromBody] UpdateOrganizationMembersCommand request)
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
		public async Task<IActionResult> Delete([FromBody] DeleteCommand<OrganizationMembers> request)
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
