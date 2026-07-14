using Job.Application.Common;
using Job.Application.Common.CRUD.Commands;
using Job.Application.Common.CRUD.Queries;
using Job.Application.Features.Job.Commands.AddJobs;
using Job.Application.Features.Job.Commands.UpdateJobs;
using Job.Application.Features.Job.Queries;
using Job.Domain.Job;
using Job.WebAPI.Controllers.Job;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Moq;
using Xunit;

namespace Job.WebAPI.Tests.Controllers.Job;

public class JobsControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly JobsController _controller;

	public JobsControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new JobsController(_mediatorMock.Object);
	}

	[Fact]
	public async Task GetAll_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetAllQuery<JobsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<JobsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<JobsResponse>());

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertOkResult<List<JobsResponse>>(result);
	}

	[Fact]
	public async Task GetAll_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetAllQuery<JobsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<JobsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<List<JobsResponse>>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Jobs")));

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertStatusCodeResult<List<JobsResponse>>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task GetById_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetByIdQuery<JobsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<JobsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new JobsResponse { Id = 1 });

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertOkResult<JobsResponse>(result);
	}

	[Fact]
	public async Task GetById_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetByIdQuery<JobsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<JobsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<JobsResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Jobs")));

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertStatusCodeResult<JobsResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Create_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new AddJobsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddJobsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Create_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new AddJobsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddJobsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.BadRequest,
				Error.EntityCouldNotBeCreated("Jobs")));

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.BadRequest);
	}

	[Fact]
	public async Task Update_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new UpdateJobsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateJobsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Update_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new UpdateJobsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateJobsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Job")));

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Delete_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new DeleteCommand<Jobs> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<Jobs>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Delete_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new DeleteCommand<Jobs> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<Jobs>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Jobs")));

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}
}
