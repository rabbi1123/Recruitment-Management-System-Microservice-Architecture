using Candidate.Application.Common;
using Candidate.Application.Common.CRUD.Commands;
using Candidate.Application.Common.CRUD.Queries;
using Candidate.Application.Features.CandidateExperience.Commands.AddCandidateExperiences;
using Candidate.Application.Features.CandidateExperience.Commands.UpdateCandidateExperiences;
using Candidate.Application.Features.CandidateExperience.Queries;
using Candidate.Domain.CandidateExperience;
using Candidate.WebAPI.Controllers.CandidateExperience;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Moq;
using Xunit;

namespace Candidate.WebAPI.Tests.Controllers.CandidateExperience;

public class CandidateExperiencesControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly CandidateExperiencesController _controller;

	public CandidateExperiencesControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new CandidateExperiencesController(_mediatorMock.Object);
	}

	[Fact]
	public async Task GetAll_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetAllQuery<CandidateExperiencesResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<CandidateExperiencesResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<CandidateExperiencesResponse>());

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertOkResult<List<CandidateExperiencesResponse>>(result);
	}

	[Fact]
	public async Task GetAll_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetAllQuery<CandidateExperiencesResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<CandidateExperiencesResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<List<CandidateExperiencesResponse>>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateExperiences")));

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertStatusCodeResult<List<CandidateExperiencesResponse>>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task GetById_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetByIdQuery<CandidateExperiencesResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<CandidateExperiencesResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CandidateExperiencesResponse { Id = 1 });

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertOkResult<CandidateExperiencesResponse>(result);
	}

	[Fact]
	public async Task GetById_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetByIdQuery<CandidateExperiencesResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<CandidateExperiencesResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CandidateExperiencesResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateExperiences")));

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertStatusCodeResult<CandidateExperiencesResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Create_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new AddCandidateExperiencesCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddCandidateExperiencesCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Create_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new AddCandidateExperiencesCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddCandidateExperiencesCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.BadRequest,
				Error.EntityCouldNotBeCreated("CandidateExperiences")));

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.BadRequest);
	}

	[Fact]
	public async Task Update_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new UpdateCandidateExperiencesCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateCandidateExperiencesCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Update_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new UpdateCandidateExperiencesCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateCandidateExperiencesCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateExperience")));

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Delete_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new DeleteCommand<CandidateExperiences> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<CandidateExperiences>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Delete_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new DeleteCommand<CandidateExperiences> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<CandidateExperiences>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateExperiences")));

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}
}
