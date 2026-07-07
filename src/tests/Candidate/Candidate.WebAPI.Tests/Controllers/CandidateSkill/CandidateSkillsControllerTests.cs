using Candidate.Application.Common;
using Candidate.Application.Common.CRUD.Commands;
using Candidate.Application.Common.CRUD.Queries;
using Candidate.Application.Features.CandidateSkill.Commands.AddCandidateSkills;
using Candidate.Application.Features.CandidateSkill.Commands.UpdateCandidateSkills;
using Candidate.Application.Features.CandidateSkill.Queries;
using Candidate.Domain.CandidateSkill;
using Candidate.WebAPI.Controllers.CandidateSkill;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Moq;
using Xunit;

namespace Candidate.WebAPI.Tests.Controllers.CandidateSkill;

public class CandidateSkillsControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly CandidateSkillsController _controller;

	public CandidateSkillsControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new CandidateSkillsController(_mediatorMock.Object);
	}

	[Fact]
	public async Task GetAll_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetAllQuery<CandidateSkillsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<CandidateSkillsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<CandidateSkillsResponse>());

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertOkResult<List<CandidateSkillsResponse>>(result);
	}

	[Fact]
	public async Task GetAll_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetAllQuery<CandidateSkillsResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<CandidateSkillsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<List<CandidateSkillsResponse>>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateSkills")));

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertStatusCodeResult<List<CandidateSkillsResponse>>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task GetById_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetByIdQuery<CandidateSkillsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<CandidateSkillsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CandidateSkillsResponse { Id = 1 });

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertOkResult<CandidateSkillsResponse>(result);
	}

	[Fact]
	public async Task GetById_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new GetByIdQuery<CandidateSkillsResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<CandidateSkillsResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CandidateSkillsResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateSkills")));

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertStatusCodeResult<CandidateSkillsResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Create_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new AddCandidateSkillsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddCandidateSkillsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Create_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new AddCandidateSkillsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddCandidateSkillsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.Conflict,
				Error.Conflict("Candidate skill with this name already exists for the candidate")));

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.Conflict);
	}

	[Fact]
	public async Task Update_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new UpdateCandidateSkillsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateCandidateSkillsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Update_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new UpdateCandidateSkillsCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateCandidateSkillsCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateSkill")));

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}

	[Fact]
	public async Task Delete_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new DeleteCommand<CandidateSkills> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<CandidateSkills>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Delete_WhenMediatorFails_ReturnsStatusCode()
	{
		var request = new DeleteCommand<CandidateSkills> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<CandidateSkills>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(Result.Failure<CommandResponse>(
				HttpResponseStatusCodes.NotFound,
				Error.NotFound("CandidateSkills")));

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertStatusCodeResult<CommandResponse>(
			result,
			HttpResponseStatusCodes.NotFound);
	}
}
