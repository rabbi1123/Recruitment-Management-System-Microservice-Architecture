using Candidate.Application.Common;
using Candidate.Application.Common.CRUD.Commands;
using Candidate.Application.Common.CRUD.Queries;
using Candidate.Application.Features.Resume.Commands.AddResumes;
using Candidate.Application.Features.Resume.Commands.UpdateResumes;
using Candidate.Application.Features.Resume.Queries;
using Candidate.Domain.Resume;
using Candidate.WebAPI.Controllers.Resume;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Moq;
using Xunit;

namespace Candidate.WebAPI.Tests.Controllers.Resume;

public class ResumesControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly ResumesController _controller;

	public ResumesControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new ResumesController(_mediatorMock.Object);
	}

	[Fact]
	public async Task GetAll_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetAllQuery<ResumesResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<ResumesResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<ResumesResponse>());

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertOkResult<List<ResumesResponse>>(result);
	}

	[Fact]
	public async Task GetAll_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetAllQuery<ResumesResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<ResumesResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<List<ResumesResponse>>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Resumes")));

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertStatusCodeResult<List<ResumesResponse>>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task GetById_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetByIdQuery<ResumesResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<ResumesResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ResumesResponse { Id = 1 });

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertOkResult<ResumesResponse>(result);
	}

	[Fact]
	public async Task GetById_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetByIdQuery<ResumesResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<ResumesResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<ResumesResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Resumes")));

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertStatusCodeResult<ResumesResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Create_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new AddResumesCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddResumesCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Create_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new AddResumesCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddResumesCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.BadRequest,
				Error.EntityCouldNotBeCreated("Resumes")));

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.BadRequest);
	}

	[Fact]
	public async Task Update_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new UpdateResumesCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateResumesCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Update_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new UpdateResumesCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateResumesCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Resume")));

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Delete_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new DeleteCommand<Resumes> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<Resumes>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Delete_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new DeleteCommand<Resumes> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<Resumes>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Resumes")));

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}
}
