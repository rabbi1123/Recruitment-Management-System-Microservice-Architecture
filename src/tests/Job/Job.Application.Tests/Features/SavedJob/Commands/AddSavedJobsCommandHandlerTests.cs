using Job.Application.Abstractions.Data;
using Job.Application.Features.SavedJob.Commands.AddSavedJobs;
using Job.Domain.SavedJob;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Job.Application.Tests.Features.SavedJob.Commands;

public class AddSavedJobsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<SavedJobs>> _repositoryMock;
	private readonly AddSavedJobsCommandHandler _handler;

	public AddSavedJobsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<SavedJobs>>();
		_handler = new AddSavedJobsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenNoConflictAndAddSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<SavedJobs>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.GetByPropertiesAsync(
				It.Is<object>(filters =>
					HasProperty(filters, "CandidateId", command.CandidateId) &&
					HasProperty(filters, "JobId", command.JobId)),
				false,
				true),
			Times.Once);

		_repositoryMock.Verify(
			r => r.AddAsync(It.Is<SavedJobs>(s =>
				s.CandidateId == command.CandidateId &&
				s.JobId == command.JobId &&
				s.CreatedBy == command.CreatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenConflictExists_ReturnsConflictWithoutAdding()
	{
		var command = CreateValidCommand();
		var existing = SavedJobs.Create(
			command.CandidateId,
			command.JobId,
			"system");

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([existing]);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.Conflict, result.Status);
		Assert.Equal(
			Error.Conflict("Saved job already exists for this candidate and job").Message,
			result.Error!.Message);

		_repositoryMock.Verify(r => r.AddAsync(It.IsAny<SavedJobs>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenAddReturnsZero_ReturnsBadRequest()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<SavedJobs>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.BadRequest, result.Status);
		Assert.Equal(Error.EntityCouldNotBeCreated("SavedJobs").Message, result.Error!.Message);
	}

	private static AddSavedJobsCommand CreateValidCommand() => new()
	{
		CandidateId = 5,
		JobId = 1,
		CreatedBy = "admin"
	};

	private static bool HasProperty(object instance, string propertyName, object? expectedValue)
	{
		var property = instance.GetType().GetProperty(propertyName);
		return property is not null && Equals(property.GetValue(instance), expectedValue);
	}
}
