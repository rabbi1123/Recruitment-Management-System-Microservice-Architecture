using Candidate.Application.Common;
using Candidate.Application.Common.CRUD.Commands;
using Candidate.Application.Common.CRUD.Queries;
using Candidate.Application.Features.CandidateCertification.Commands.AddCandidateCertifications;
using Candidate.Application.Features.CandidateCertification.Commands.UpdateCandidateCertifications;
using Candidate.Application.Features.CandidateCertification.Queries;
using Candidate.Domain.CandidateCertification;
using Candidate.WebAPI.Controllers.CandidateCertification;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Moq;
using Xunit;

namespace Candidate.WebAPI.Tests.Controllers.CandidateCertification;

public class CandidateCertificationsControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly CandidateCertificationsController _controller;

	public CandidateCertificationsControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new CandidateCertificationsController(_mediatorMock.Object);
	}

	[Fact]
	public async Task GetAll_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetAllQuery<CandidateCertificationsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<CandidateCertificationsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<CandidateCertificationsResponse>());

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertOkResult<List<CandidateCertificationsResponse>>(result);
	}

	[Fact]
	public async Task GetAll_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetAllQuery<CandidateCertificationsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<CandidateCertificationsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<List<CandidateCertificationsResponse>>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateCertifications")));

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertStatusCodeResult<List<CandidateCertificationsResponse>>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task GetById_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetByIdQuery<CandidateCertificationsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<CandidateCertificationsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CandidateCertificationsResponse { Id = 1 });

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertOkResult<CandidateCertificationsResponse>(result);
	}

	[Fact]
	public async Task GetById_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetByIdQuery<CandidateCertificationsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<CandidateCertificationsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CandidateCertificationsResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateCertifications")));

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertStatusCodeResult<CandidateCertificationsResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Create_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new AddCandidateCertificationsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddCandidateCertificationsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Create_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new AddCandidateCertificationsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddCandidateCertificationsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.BadRequest,
				Error.EntityCouldNotBeCreated("CandidateCertifications")));

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.BadRequest);
	}

	[Fact]
	public async Task Update_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new UpdateCandidateCertificationsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateCandidateCertificationsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Update_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new UpdateCandidateCertificationsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateCandidateCertificationsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateCertification")));

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Delete_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new DeleteCommand<CandidateCertifications> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<CandidateCertifications>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Delete_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new DeleteCommand<CandidateCertifications> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<CandidateCertifications>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateCertifications")));

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}
}
