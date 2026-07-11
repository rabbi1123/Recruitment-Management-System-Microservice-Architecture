using Organization.Application.Common;
using Organization.Application.Common.CRUD.Commands;
using Organization.Application.Common.CRUD.Queries;
using Organization.Application.Features.OrganizationMember.Commands.AddOrganizationMembers;
using Organization.Application.Features.OrganizationMember.Commands.UpdateOrganizationMembers;
using Organization.Application.Features.OrganizationMember.Queries;
using Organization.Domain.OrganizationMember;
using Organization.WebAPI.Controllers.OrganizationMember;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Moq;
using Xunit;

namespace Organization.WebAPI.Tests.Controllers.OrganizationMember;

public class OrganizationMembersControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly OrganizationMembersController _controller;

	public OrganizationMembersControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new OrganizationMembersController(_mediatorMock.Object);
	}

	[Fact]
	public async Task GetAll_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetAllQuery<OrganizationMembersResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<OrganizationMembersResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<OrganizationMembersResponse>());

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertOkResult<List<OrganizationMembersResponse>>(result);
	}

	[Fact]
	public async Task GetAll_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetAllQuery<OrganizationMembersResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<OrganizationMembersResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<List<OrganizationMembersResponse>>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("OrganizationMembers")));

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertStatusCodeResult<List<OrganizationMembersResponse>>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task GetById_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetByIdQuery<OrganizationMembersResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<OrganizationMembersResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new OrganizationMembersResponse { Id = 1 });

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertOkResult<OrganizationMembersResponse>(result);
	}

	[Fact]
	public async Task GetById_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetByIdQuery<OrganizationMembersResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<OrganizationMembersResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<OrganizationMembersResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("OrganizationMembers")));

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertStatusCodeResult<OrganizationMembersResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Create_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new AddOrganizationMembersCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddOrganizationMembersCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Create_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new AddOrganizationMembersCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddOrganizationMembersCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.Conflict,
				Error.Conflict("Organization member with this user already exists for the organization")));

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.Conflict);
	}

	[Fact]
	public async Task Update_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new UpdateOrganizationMembersCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateOrganizationMembersCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Update_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new UpdateOrganizationMembersCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateOrganizationMembersCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("OrganizationMember")));

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Delete_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new DeleteCommand<OrganizationMembers> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<OrganizationMembers>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Delete_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new DeleteCommand<OrganizationMembers> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<OrganizationMembers>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("OrganizationMembers")));

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}
}
