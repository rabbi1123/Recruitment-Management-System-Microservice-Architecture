using Candidate.Application.Abstractions.Data;
using Candidate.Application.Features.CandidateSkill.Commands.UpdateCandidateSkills;
using Candidate.Domain.CandidateSkill;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Candidate.Application.Tests.Features.CandidateSkill.Commands;

public class UpdateCandidateSkillsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<CandidateSkills>> _repositoryMock;
	private readonly UpdateCandidateSkillsCommandHandler _handler;

	public UpdateCandidateSkillsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<CandidateSkills>>();
		_handler = new UpdateCandidateSkillsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenNoConflictAndUpdateSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingSkill(command);

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<CandidateSkills>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.UpdateAsync(It.Is<CandidateSkills>(s =>
				s.Name == command.Name &&
				s.ProficiencyLevel == command.ProficiencyLevel &&
				s.YearsOfExperience == command.YearsOfExperience &&
				s.IsActive == command.IsActive &&
				s.UpdatedBy == command.UpdatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenOnlyMatchingSkillIsSameId_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingSkill(command);

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([existing]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<CandidateSkills>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CandidateSkills>()), Times.Once);
	}

	[Fact]
	public async Task Handle_WhenConflictExistsWithDifferentId_ReturnsConflictWithoutUpdating()
	{
		var command = CreateValidCommand();
		var conflictingSkill = CandidateSkills.Create(
			command.CandidateId,
			command.Name,
			"Intermediate",
			3,
			"system");
		conflictingSkill.Id = command.Id + 1;

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([conflictingSkill]);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.Conflict, result.Status);
		Assert.Equal(
			Error.Conflict("Candidate skill with this name already exists for the candidate").Message,
			result.Error!.Message);

		_repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CandidateSkills>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenSkillNotFound_ReturnsNotFound()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync((CandidateSkills?)null!);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("CandidateSkill").Message, result.Error!.Message);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CandidateSkills>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenUpdateReturnsZero_ReturnsNotFound()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingSkill(command);

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<CandidateSkills>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("CandidateSkills").Message, result.Error!.Message);
	}

	private static UpdateCandidateSkillsCommand CreateValidCommand() => new()
	{
		Id = 10,
		CandidateId = 1,
		Name = "C#",
		ProficiencyLevel = "Expert",
		YearsOfExperience = 7,
		IsActive = true,
		UpdatedBy = "admin"
	};

	private static CandidateSkills CreateExistingSkill(UpdateCandidateSkillsCommand command)
	{
		var skill = CandidateSkills.Create(
			command.CandidateId,
			"JavaScript",
			"Intermediate",
			2,
			"creator");

		skill.Id = command.Id;
		skill.IsActive = false;

		return skill;
	}
}
