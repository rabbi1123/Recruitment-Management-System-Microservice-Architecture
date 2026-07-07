using Candidate.Application.Common;
using Candidate.Application.Common.CRUD.Commands;
using Candidate.Application.Common.CRUD.Queries;
using Candidate.Application.Features.CandidateEducation.Commands.AddCandidateEducations;
using Candidate.Application.Features.CandidateEducation.Commands.UpdateCandidateEducations;
using Candidate.Application.Features.CandidateEducation.Queries;
using Candidate.Domain.CandidateEducation;
using Candidate.WebAPI.Controllers.CandidateEducation;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Moq;
using Xunit;

namespace Candidate.WebAPI.Tests.Controllers.CandidateEducation;

public class CandidateEducationsControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly CandidateEducationsController _controller;

	public CandidateEducationsControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new CandidateEducationsController(_mediatorMock.Object);
	}

	[Fact]
	public async Task GetAll_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetAllQuery<CandidateEducationsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<CandidateEducationsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<CandidateEducationsResponse>());

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertOkResult<List<CandidateEducationsResponse>>(result);
	}

	[Fact]
	public async Task GetAll_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetAllQuery<CandidateEducationsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<CandidateEducationsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<List<CandidateEducationsResponse>>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateEducations")));

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertStatusCodeResult<List<CandidateEducationsResponse>>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task GetById_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetByIdQuery<CandidateEducationsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<CandidateEducationsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CandidateEducationsResponse { Id = 1 });

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertOkResult<CandidateEducationsResponse>(result);
	}

	[Fact]
	public async Task GetById_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetByIdQuery<CandidateEducationsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<CandidateEducationsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CandidateEducationsResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateEducations")));

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertStatusCodeResult<CandidateEducationsResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Create_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new AddCandidateEducationsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddCandidateEducationsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Create_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new AddCandidateEducationsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddCandidateEducationsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.BadRequest,
				Error.EntityCouldNotBeCreated("CandidateEducations")));

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.BadRequest);
	}

	[Fact]
	public async Task Update_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new UpdateCandidateEducationsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateCandidateEducationsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Update_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new UpdateCandidateEducationsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateCandidateEducationsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateEducation")));

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Delete_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new DeleteCommand<CandidateEducations> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<CandidateEducations>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Delete_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new DeleteCommand<CandidateEducations> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<CandidateEducations>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateEducations")));

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}
}
