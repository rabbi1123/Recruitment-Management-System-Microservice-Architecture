using Candidate.Application.Common;
using Candidate.Application.Common.CRUD.Commands;
using Candidate.Application.Common.CRUD.Queries;
using Candidate.Application.Features.Candidate.Commands.AddCandidates;
using Candidate.Application.Features.Candidate.Commands.UpdateCandidates;
using Candidate.Application.Features.Candidate.Queries;
using Candidate.Domain.Candidate;
using Candidate.WebAPI.Controllers.Candidate;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Moq;
using Xunit;

namespace Candidate.WebAPI.Tests.Controllers.Candidate;

public class CandidatesControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly CandidatesController _controller;

	public CandidatesControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new CandidatesController(_mediatorMock.Object);
	}

	[Fact]
	public async Task GetAll_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetAllQuery<CandidatesResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<CandidatesResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<CandidatesResponse>());

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertOkResult<List<CandidatesResponse>>(result);
	}

	[Fact]
	public async Task GetAll_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetAllQuery<CandidatesResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<CandidatesResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<List<CandidatesResponse>>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Candidates")));

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertStatusCodeResult<List<CandidatesResponse>>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task GetById_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetByIdQuery<CandidatesResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<CandidatesResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CandidatesResponse { Id = 1 });

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertOkResult<CandidatesResponse>(result);
	}

	[Fact]
	public async Task GetById_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetByIdQuery<CandidatesResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<CandidatesResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CandidatesResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Candidates")));

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertStatusCodeResult<CandidatesResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Create_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new AddCandidatesCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddCandidatesCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Create_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new AddCandidatesCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddCandidatesCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.Conflict,
				Error.Conflict("Candidates with this user id or email")));

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.Conflict);
	}

	[Fact]
	public async Task Update_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new UpdateCandidatesCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateCandidatesCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Update_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new UpdateCandidatesCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateCandidatesCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Candidate")));

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Delete_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new DeleteCommand<Candidates> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<Candidates>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Delete_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new DeleteCommand<Candidates> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<Candidates>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("Candidates")));

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}
}
