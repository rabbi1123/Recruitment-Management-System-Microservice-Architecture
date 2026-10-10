using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Notification.WebAPI.Controllers;

[ApiController]
[Route("/")]
public class ApiBaseController : ControllerBase
{
	private readonly IMediator _mediator;

	public ApiBaseController(IMediator mediator)
	{
		_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
	}

	protected async Task<TResult> QueryAsync<TResult>(IRequest<TResult> query)
	{
		return await _mediator.Send(query);
	}
}
