using Candidate.Application.Abstractions.Data;
using Candidate.Application.Features.Candidate.Commands.UpdateCandidates;
using Candidate.Domain.Candidate;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Candidate.Application.Tests.Features.Candidate.Commands;

public class UpdateCandidatesCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Candidates>> _repositoryMock;
	private readonly UpdateCandidatesCommandHandler _handler;

	public UpdateCandidatesCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Candidates>>();
		_handler = new UpdateCandidatesCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenNoConflictAndUpdateSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existingCandidate = CreateExistingCandidate(command);

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), true, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existingCandidate);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<Candidates>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.False(result.IsFailure);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.GetByPropertiesAsync(
				It.Is<object>(filters =>
					HasProperty(filters, "UserId", command.UserId) &&
					HasProperty(filters, "Email", command.Email)),
				true,
				true),
			Times.Once);

		_repositoryMock.Verify(r => r.GetByIdAsync(command.Id), Times.Once);

		_repositoryMock.Verify(
			r => r.UpdateAsync(It.Is<Candidates>(c =>
				c.Id == command.Id &&
				c.FirstName == command.FirstName &&
				c.LastName == command.LastName &&
				c.Email == command.Email &&
				c.Phone == command.Phone &&
				c.Address == command.Address &&
				c.LinkedinProfile == command.LinkedinProfile &&
				c.PortfolioUrl == command.PortfolioUrl &&
				c.Headline == command.Headline &&
				c.Summary == command.Summary &&
				c.IsActive == command.IsActive &&
				c.UpdatedBy == command.UpdatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenOnlyMatchingCandidateIsSameId_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existingCandidate = CreateExistingCandidate(command);

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), true, true))
			.ReturnsAsync([existingCandidate]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existingCandidate);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<Candidates>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Candidates>()), Times.Once);
	}

	[Fact]
	public async Task Handle_WhenConflictExistsWithDifferentId_ReturnsConflictWithoutUpdating()
	{
		var command = CreateValidCommand();
		var conflictingCandidate = Candidates.Create(
			command.UserId,
			"Other",
			"Candidate",
			command.Email,
			null,
			null,
			null,
			null,
			null,
			null,
			"system");
		conflictingCandidate.Id = command.Id + 1;

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), true, true))
			.ReturnsAsync([conflictingCandidate]);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.False(result.IsSuccess);
		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.Conflict, result.Status);
		Assert.NotNull(result.Error);
		Assert.Equal(Error.Conflict("Candidates with this id or email").Message, result.Error!.Message);

		_repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Candidates>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenCandidateNotFound_ReturnsNotFound()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), true, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync((Candidates?)null!);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.False(result.IsSuccess);
		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.NotNull(result.Error);
		Assert.Equal(Error.NotFound("Candidate").Message, result.Error!.Message);

		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Candidates>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenUpdateReturnsZero_ReturnsNotFound()
	{
		var command = CreateValidCommand();
		var existingCandidate = CreateExistingCandidate(command);

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), true, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existingCandidate);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<Candidates>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.False(result.IsSuccess);
		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.NotNull(result.Error);
		Assert.Equal(Error.NotFound("Candidates").Message, result.Error!.Message);
	}

	private static UpdateCandidatesCommand CreateValidCommand() => new()
	{
		Id = 10,
		UserId = 42,
		FirstName = "Jane",
		LastName = "Doe",
		Email = "jane.doe@example.com",
		Phone = "555-0100",
		Address = "123 Main St",
		LinkedinProfile = "https://linkedin.com/in/janedoe",
		PortfolioUrl = "https://janedoe.dev",
		Headline = "Senior Software Engineer",
		Summary = "Experienced developer",
		IsActive = true,
		UpdatedBy = "admin"
	};

	private static Candidates CreateExistingCandidate(UpdateCandidatesCommand command)
	{
		var candidate = Candidates.Create(
			command.UserId,
			"Old",
			"Name",
			"old.email@example.com",
			"000-0000",
			"Old Address",
			null,
			null,
			"Old Headline",
			"Old Summary",
			"creator");

		candidate.Id = command.Id;
		candidate.IsActive = false;

		return candidate;
	}

	private static bool HasProperty(object instance, string propertyName, object? expectedValue)
	{
		var property = instance.GetType().GetProperty(propertyName);
		return property is not null && Equals(property.GetValue(instance), expectedValue);
	}
}
