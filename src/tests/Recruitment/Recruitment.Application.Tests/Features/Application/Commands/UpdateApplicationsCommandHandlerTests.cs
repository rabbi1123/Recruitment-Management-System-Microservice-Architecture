using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Features.Application.Commands.UpdateApplications;
using Recruitment.Domain.Application;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Recruitment.Application.Tests.Features.Application.Commands;

public class UpdateApplicationsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Applications>> _repositoryMock;
	private readonly UpdateApplicationsCommandHandler _handler;

	public UpdateApplicationsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Applications>>();
		_handler = new UpdateApplicationsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenUpdateSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingApplication(command);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<Applications>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.UpdateAsync(It.Is<Applications>(a =>
				a.Id == command.Id &&
				a.RecruiterId == command.RecruiterId &&
				a.CurrentStage == command.CurrentStage &&
				a.Status == command.Status &&
				a.CoverLetter == command.CoverLetter &&
				a.RejectionReason == command.RejectionReason &&
				a.IsActive == command.IsActive &&
				a.UpdatedBy == command.UpdatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenApplicationNotFound_ReturnsNotFound()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync((Applications?)null!);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("Application").Message, result.Error!.Message);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Applications>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenUpdateReturnsZero_ReturnsNotFound()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingApplication(command);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<Applications>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("Applications").Message, result.Error!.Message);
	}

	private static UpdateApplicationsCommand CreateValidCommand() => new()
	{
		Id = 10,
		RecruiterId = 5,
		CurrentStage = "UnderReview",
		Status = "Active",
		CoverLetter = "Updated cover letter",
		RejectionReason = null,
		IsActive = true,
		UpdatedBy = "recruiter"
	};

	private static Applications CreateExistingApplication(UpdateApplicationsCommand command)
	{
		var application = Applications.Create(1, 2, 3, null, "Original cover letter", "creator");
		application.Id = command.Id;
		return application;
	}
}
