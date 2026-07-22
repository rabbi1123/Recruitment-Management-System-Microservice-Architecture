using Recruitment.Application.Common;
using Recruitment.Application.Common.CRUD.Commands;
using Recruitment.Application.Common.CRUD.Queries;
using Recruitment.Application.Features.Application.Commands.AddApplications;
using Recruitment.Application.Features.Application.Commands.UpdateApplications;
using Recruitment.Application.Features.Application.Queries;
using Recruitment.Domain.Application;
using Recruitment.WebAPI.Controllers.Application;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Moq;
using Xunit;

namespace Recruitment.WebAPI.Tests.Controllers.Application;

public class ApplicationsControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly ApplicationsController _controller;

	public ApplicationsControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new ApplicationsController(_mediatorMock.Object);
	}

	[Fact]
	public async Task GetAll_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetAllQuery<ApplicationsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<ApplicationsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<ApplicationsResponse>());

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertOkResult<List<ApplicationsResponse>>(result);
	}

	[Fact]
	public async Task GetById_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetByIdQuery<ApplicationsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<ApplicationsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ApplicationsResponse { Id = 1 });

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertOkResult<ApplicationsResponse>(result);
	}

	[Fact]
	public async Task Create_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new AddApplicationsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddApplicationsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Create_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new AddApplicationsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddApplicationsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.Conflict,
				Error.Conflict("Application already exists for this job and candidate")));

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.Conflict);
	}

	[Fact]
	public async Task Update_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new UpdateApplicationsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateApplicationsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Delete_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new DeleteCommand<Applications> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<Applications>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}
}
