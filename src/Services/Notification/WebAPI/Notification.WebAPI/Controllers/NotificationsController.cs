using Common.Platform.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.Common.CRUD.Queries;
using Notification.Application.Features.Notifications.Queries;

namespace Notification.WebAPI.Controllers;

[Route(ApiRoutes.Notification.Notifications)]
public class NotificationsController : ApiBaseController
{
	public NotificationsController(IMediator mediator) : base(mediator)
	{
	}

	[HttpPost(ApiRoutes.Common.GetAll)]
	[ProducesResponseType(typeof(Result<List<NotificationsResponse>>), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(Result<List<NotificationsResponse>>), StatusCodes.Status400BadRequest, "application/json")]
	public async Task<IActionResult> GetAll(GetAllQuery<NotificationsResponse> request)
	{
		var response = await QueryAsync(request);

		if (response.IsFailure)
			return StatusCode(response.Status, response);

		return Ok(response);
	}

	[HttpPost(ApiRoutes.Common.GetById)]
	[ProducesResponseType(typeof(Result<NotificationsResponse>), StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType(typeof(Result<NotificationsResponse>), StatusCodes.Status400BadRequest, "application/json")]
	[ProducesResponseType(typeof(Result<NotificationsResponse>), StatusCodes.Status404NotFound, "application/json")]
	public async Task<IActionResult> GetById([FromBody] GetByIdQuery<NotificationsResponse> request)
	{
		var response = await QueryAsync(request);

		if (response.IsFailure)
			return StatusCode(response.Status, response);

		return Ok(response);
	}
}
