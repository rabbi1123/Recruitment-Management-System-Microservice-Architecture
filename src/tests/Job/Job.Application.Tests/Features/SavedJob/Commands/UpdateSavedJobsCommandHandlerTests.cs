using Job.Application.Abstractions.Data;
using Job.Application.Features.SavedJob.Commands.UpdateSavedJobs;
using Job.Domain.SavedJob;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Job.Application.Tests.Features.SavedJob.Commands;

public class UpdateSavedJobsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<SavedJobs>> _repositoryMock;
	private readonly UpdateSavedJobsCommandHandler _handler;

	public UpdateSavedJobsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<SavedJobs>>();
		_handler = new UpdateSavedJobsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenNoConflictAndUpdateSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingSavedJob(command);

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<SavedJobs>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.UpdateAsync(It.Is<SavedJobs>(s =>
				s.IsActive == command.IsActive &&
				s.UpdatedBy == command.UpdatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenOnlyMatchingSavedJobIsSameId_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingSavedJob(command);

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([existing]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<SavedJobs>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SavedJobs>()), Times.Once);
	}

	[Fact]
	public async Task Handle_WhenConflictExistsWithDifferentId_ReturnsConflictWithoutUpdating()
	{
		var command = CreateValidCommand();
		var conflicting = SavedJobs.Create(
			command.CandidateId,
			command.JobId,
			"system");
		conflicting.Id = command.Id + 1;

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([conflicting]);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.Conflict, result.Status);
		Assert.Equal(
			Error.Conflict("Saved job already exists for this candidate and job").Message,
			result.Error!.Message);

		_repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SavedJobs>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenSavedJobNotFound_ReturnsNotFound()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync((SavedJobs?)null!);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("SavedJob").Message, result.Error!.Message);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SavedJobs>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenUpdateReturnsZero_ReturnsNotFound()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingSavedJob(command);

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<SavedJobs>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("SavedJobs").Message, result.Error!.Message);
	}

	private static UpdateSavedJobsCommand CreateValidCommand() => new()
	{
		Id = 10,
		CandidateId = 5,
		JobId = 1,
		IsActive = true,
		UpdatedBy = "admin"
	};

	private static SavedJobs CreateExistingSavedJob(UpdateSavedJobsCommand command)
	{
		var savedJob = SavedJobs.Create(
			command.CandidateId,
			command.JobId,
			"creator");

		savedJob.Id = command.Id;
		savedJob.IsActive = false;

		return savedJob;
	}
}
