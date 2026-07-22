using Recruitment.Application.Common;
using Recruitment.Application.Common.CRUD.Commands;
using Recruitment.Application.Common.CRUD.Queries;
using Recruitment.Application.Features.ApplicationComment.Commands.AddApplicationComments;
using Recruitment.Application.Features.ApplicationComment.Commands.UpdateApplicationComments;
using Recruitment.Application.Features.ApplicationComment.Queries;
using Recruitment.Domain.ApplicationComment;
using Recruitment.WebAPI.Controllers.ApplicationComment;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Moq;
using Xunit;

namespace Recruitment.WebAPI.Tests.Controllers.ApplicationComment;

public class ApplicationCommentsControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly ApplicationCommentsController _controller;

	public ApplicationCommentsControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new ApplicationCommentsController(_mediatorMock.Object);
	}

	[Fact]
	public async Task GetAll_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetAllQuery<ApplicationCommentsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<ApplicationCommentsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<ApplicationCommentsResponse>());

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertOkResult<List<ApplicationCommentsResponse>>(result);
	}

	[Fact]
	public async Task GetById_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetByIdQuery<ApplicationCommentsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<ApplicationCommentsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ApplicationCommentsResponse { Id = 1 });

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertOkResult<ApplicationCommentsResponse>(result);
	}

	[Fact]
	public async Task Create_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new AddApplicationCommentsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddApplicationCommentsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Update_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new UpdateApplicationCommentsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateApplicationCommentsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Delete_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new DeleteCommand<ApplicationComments> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<ApplicationComments>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}
}
