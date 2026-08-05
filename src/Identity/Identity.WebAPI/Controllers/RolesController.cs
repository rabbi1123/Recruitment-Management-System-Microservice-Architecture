using Identity.Application.Features.Roles.AssignRole;
using Identity.Application.Features.Roles.GetUserRoles;
using Identity.Application.Features.Roles.RemoveRole;
using Identity.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.WebAPI.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize(Roles = Roles.Admin)]
public sealed class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("assign")]
    public async Task<IActionResult> Assign([FromBody] AssignRoleCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove([FromBody] RemoveRoleCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpGet("user/{id:guid}")]
    public async Task<IActionResult> GetUserRoles(Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetUserRolesQuery(id), cancellationToken);
        return Ok(response);
    }
}
