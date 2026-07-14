using Job.Application.Abstractions.Data;
using Job.Application.Features.JobSkill.Commands.AddJobSkills;
using Job.Domain.JobSkill;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Job.Application.Tests.Features.JobSkill.Commands;

public class AddJobSkillsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<JobSkills>> _repositoryMock;
	private readonly AddJobSkillsCommandHandler _handler;

	public AddJobSkillsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<JobSkills>>();
		_handler = new AddJobSkillsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenNoConflictAndAddSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<JobSkills>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.GetByPropertiesAsync(
				It.Is<object>(filters =>
					HasProperty(filters, "JobId", command.JobId) &&
					HasProperty(filters, "SkillName", command.SkillName)),
				false,
				true),
			Times.Once);

		_repositoryMock.Verify(
			r => r.AddAsync(It.Is<JobSkills>(s =>
				s.JobId == command.JobId &&
				s.SkillName == command.SkillName &&
				s.IsRequired == command.IsRequired &&
				s.CreatedBy == command.CreatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenConflictExists_ReturnsConflictWithoutAdding()
	{
		var command = CreateValidCommand();
		var existingSkill = JobSkills.Create(
			command.JobId,
			command.SkillName,
			true,
			"system");

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([existingSkill]);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.Conflict, result.Status);
		Assert.Equal(
			Error.Conflict("Job skill with this name already exists for the job").Message,
			result.Error!.Message);

		_repositoryMock.Verify(r => r.AddAsync(It.IsAny<JobSkills>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenAddReturnsZero_ReturnsBadRequest()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<JobSkills>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.BadRequest, result.Status);
		Assert.Equal(Error.EntityCouldNotBeCreated("JobSkills").Message, result.Error!.Message);
	}

	private static AddJobSkillsCommand CreateValidCommand() => new()
	{
		JobId = 1,
		SkillName = "C Sharp",
		IsRequired = true,
		CreatedBy = "admin"
	};

	private static bool HasProperty(object instance, string propertyName, object? expectedValue)
	{
		var property = instance.GetType().GetProperty(propertyName);
		return property is not null && Equals(property.GetValue(instance), expectedValue);
	}
}
