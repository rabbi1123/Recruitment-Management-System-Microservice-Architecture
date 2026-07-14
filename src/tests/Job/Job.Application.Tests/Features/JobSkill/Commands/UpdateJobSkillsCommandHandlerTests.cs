using Job.Application.Abstractions.Data;
using Job.Application.Features.JobSkill.Commands.UpdateJobSkills;
using Job.Domain.JobSkill;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Job.Application.Tests.Features.JobSkill.Commands;

public class UpdateJobSkillsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<JobSkills>> _repositoryMock;
	private readonly UpdateJobSkillsCommandHandler _handler;

	public UpdateJobSkillsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<JobSkills>>();
		_handler = new UpdateJobSkillsCommandHandler(_repositoryMock.Object);
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
			.Setup(r => r.UpdateAsync(It.IsAny<JobSkills>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.UpdateAsync(It.Is<JobSkills>(s =>
				s.SkillName == command.SkillName &&
				s.IsRequired == command.IsRequired &&
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
			.Setup(r => r.UpdateAsync(It.IsAny<JobSkills>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<JobSkills>()), Times.Once);
	}

	[Fact]
	public async Task Handle_WhenConflictExistsWithDifferentId_ReturnsConflictWithoutUpdating()
	{
		var command = CreateValidCommand();
		var conflictingSkill = JobSkills.Create(
			command.JobId,
			command.SkillName,
			true,
			"system");
		conflictingSkill.Id = command.Id + 1;

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([conflictingSkill]);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.Conflict, result.Status);
		Assert.Equal(
			Error.Conflict("Job skill with this name already exists for the job").Message,
			result.Error!.Message);

		_repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<JobSkills>()), Times.Never);
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
			.ReturnsAsync((JobSkills?)null!);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("JobSkill").Message, result.Error!.Message);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<JobSkills>()), Times.Never);
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
			.Setup(r => r.UpdateAsync(It.IsAny<JobSkills>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("JobSkills").Message, result.Error!.Message);
	}

	private static UpdateJobSkillsCommand CreateValidCommand() => new()
	{
		Id = 10,
		JobId = 1,
		SkillName = "C Sharp",
		IsRequired = true,
		IsActive = true,
		UpdatedBy = "admin"
	};

	private static JobSkills CreateExistingSkill(UpdateJobSkillsCommand command)
	{
		var skill = JobSkills.Create(
			command.JobId,
			"Old Skill",
			false,
			"creator");

		skill.Id = command.Id;
		skill.IsActive = false;

		return skill;
	}
}
