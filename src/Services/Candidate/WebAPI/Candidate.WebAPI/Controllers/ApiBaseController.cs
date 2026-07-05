using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Candidate.WebAPI.Controllers
{
	//[Authorize(Policy = "HasUserId")]
	[ApiController]
	[Route("/")]
	public class ApiBaseController : ControllerBase
	{
		private readonly IMediator _mediator;
		public ApiBaseController(IMediator mediator)
		{
			_mediator = mediator ?? throw new ArgumentNullException();
		}

		protected async Task<TResult> QueryAsync<TResult>(IRequest<TResult> query)
		{
			return await _mediator.Send(query);
		}

		protected async Task<TResult> CommandAsync<TResult>(IRequest<TResult> command)
		{
			return await _mediator.Send(command);
		}
	}
}
