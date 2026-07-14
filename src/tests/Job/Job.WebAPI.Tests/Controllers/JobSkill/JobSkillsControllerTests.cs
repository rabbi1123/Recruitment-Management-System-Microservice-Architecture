using Job.Application.Common;
using Job.Application.Common.CRUD.Commands;
using Job.Application.Common.CRUD.Queries;
using Job.Application.Features.JobSkill.Commands.AddJobSkills;
using Job.Application.Features.JobSkill.Commands.UpdateJobSkills;
using Job.Application.Features.JobSkill.Queries;
using Job.Domain.JobSkill;
using Job.WebAPI.Controllers.JobSkill;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Moq;
using Xunit;

namespace Job.WebAPI.Tests.Controllers.JobSkill;

public class JobSkillsControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly JobSkillsController _controller;

	public JobSkillsControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new JobSkillsController(_mediatorMock.Object);
	}

	[Fact]
	public async Task GetAll_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetAllQuery<JobSkillsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<JobSkillsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<JobSkillsResponse>());

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertOkResult<List<JobSkillsResponse>>(result);
	}

	[Fact]
	public async Task GetAll_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetAllQuery<JobSkillsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<JobSkillsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<List<JobSkillsResponse>>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("JobSkills")));

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertStatusCodeResult<List<JobSkillsResponse>>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task GetById_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetByIdQuery<JobSkillsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<JobSkillsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new JobSkillsResponse { Id = 1 });

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertOkResult<JobSkillsResponse>(result);
	}

	[Fact]
	public async Task GetById_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetByIdQuery<JobSkillsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<JobSkillsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<JobSkillsResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("JobSkills")));

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertStatusCodeResult<JobSkillsResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Create_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new AddJobSkillsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddJobSkillsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Create_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new AddJobSkillsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddJobSkillsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.Conflict,
				Error.Conflict("Job skill with this name already exists for the job")));

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.Conflict);
	}

	[Fact]
	public async Task Update_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new UpdateJobSkillsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateJobSkillsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Update_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new UpdateJobSkillsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateJobSkillsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("JobSkill")));

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Delete_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new DeleteCommand<JobSkills> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<JobSkills>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Delete_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new DeleteCommand<JobSkills> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<JobSkills>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("JobSkills")));

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}
}
