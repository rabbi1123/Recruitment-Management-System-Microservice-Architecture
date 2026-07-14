using Job.Application.Common;
using Job.Application.Common.CRUD.Commands;
using Job.Application.Common.CRUD.Queries;
using Job.Application.Features.SavedJob.Commands.AddSavedJobs;
using Job.Application.Features.SavedJob.Commands.UpdateSavedJobs;
using Job.Application.Features.SavedJob.Queries;
using Job.Domain.SavedJob;
using Job.WebAPI.Controllers.SavedJob;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Moq;
using Xunit;

namespace Job.WebAPI.Tests.Controllers.SavedJob;

public class SavedJobsControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly SavedJobsController _controller;

	public SavedJobsControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new SavedJobsController(_mediatorMock.Object);
	}

	[Fact]
	public async Task GetAll_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetAllQuery<SavedJobsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<SavedJobsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<SavedJobsResponse>());

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertOkResult<List<SavedJobsResponse>>(result);
	}

	[Fact]
	public async Task GetAll_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetAllQuery<SavedJobsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<SavedJobsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<List<SavedJobsResponse>>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("SavedJobs")));

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertStatusCodeResult<List<SavedJobsResponse>>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task GetById_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetByIdQuery<SavedJobsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<SavedJobsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new SavedJobsResponse { Id = 1 });

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertOkResult<SavedJobsResponse>(result);
	}

	[Fact]
	public async Task GetById_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetByIdQuery<SavedJobsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<SavedJobsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<SavedJobsResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("SavedJobs")));

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertStatusCodeResult<SavedJobsResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Create_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new AddSavedJobsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddSavedJobsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Create_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new AddSavedJobsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddSavedJobsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.Conflict,
				Error.Conflict("Saved job already exists for this candidate and job")));

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.Conflict);
	}

	[Fact]
	public async Task Update_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new UpdateSavedJobsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateSavedJobsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Update_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new UpdateSavedJobsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateSavedJobsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("SavedJob")));

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Delete_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new DeleteCommand<SavedJobs> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<SavedJobs>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Delete_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new DeleteCommand<SavedJobs> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<SavedJobs>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("SavedJobs")));

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}
}
