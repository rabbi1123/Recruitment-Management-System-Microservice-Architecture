using Organization.Application.Common;
using Organization.Application.Common.CRUD.Commands;
using Organization.Application.Common.CRUD.Queries;
using Organization.Application.Features.Organization.Commands.AddOrganizations;
using Organization.Application.Features.Organization.Commands.UpdateOrganizations;
using Organization.Application.Features.Organization.Queries;
using Organization.Domain.Organization;
using Organization.WebAPI.Controllers.Organization;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Moq;
using Xunit;

namespace Organization.WebAPI.Tests.Controllers.Organization;

public class OrganizationsControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly OrganizationsController _controller;

	public OrganizationsControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new OrganizationsController(_mediatorMock.Object);
	}

	[Fact]
	public async Task GetAll_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetAllQuery<OrganizationsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<OrganizationsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<OrganizationsResponse>());

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertOkResult<List<OrganizationsResponse>>(result);
	}

	[Fact]
	public async Task GetAll_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetAllQuery<OrganizationsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<OrganizationsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<List<OrganizationsResponse>>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Organizations")));

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertStatusCodeResult<List<OrganizationsResponse>>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task GetById_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetByIdQuery<OrganizationsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<OrganizationsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new OrganizationsResponse { Id = 1 });

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertOkResult<OrganizationsResponse>(result);
	}

	[Fact]
	public async Task GetById_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetByIdQuery<OrganizationsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<OrganizationsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<OrganizationsResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Organizations")));

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertStatusCodeResult<OrganizationsResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Create_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new AddOrganizationsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddOrganizationsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Create_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new AddOrganizationsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddOrganizationsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.BadRequest,
				Error.EntityCouldNotBeCreated("Organizations")));

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.BadRequest);
	}

	[Fact]
	public async Task Update_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new UpdateOrganizationsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateOrganizationsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Update_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new UpdateOrganizationsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateOrganizationsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Organization")));

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Delete_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new DeleteCommand<Organizations> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<Organizations>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Delete_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new DeleteCommand<Organizations> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<Organizations>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Organizations")));

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}
}
