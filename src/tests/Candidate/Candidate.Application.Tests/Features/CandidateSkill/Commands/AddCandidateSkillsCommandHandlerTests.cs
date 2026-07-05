using Candidate.Application.Abstractions.Data;
using Candidate.Application.Features.CandidateSkill.Commands.AddCandidateSkills;
using Candidate.Domain.CandidateSkill;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Candidate.Application.Tests.Features.CandidateSkill.Commands;

public class AddCandidateSkillsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<CandidateSkills>> _repositoryMock;
	private readonly AddCandidateSkillsCommandHandler _handler;

	public AddCandidateSkillsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<CandidateSkills>>();
		_handler = new AddCandidateSkillsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenNoConflictAndAddSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<CandidateSkills>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.GetByPropertiesAsync(
				It.Is<object>(filters =>
					HasProperty(filters, "CandidateId", command.CandidateId) &&
					HasProperty(filters, "Name", command.Name)),
				false,
				true),
			Times.Once);

		_repositoryMock.Verify(
			r => r.AddAsync(It.Is<CandidateSkills>(s =>
				s.CandidateId == command.CandidateId &&
				s.Name == command.Name &&
				s.ProficiencyLevel == command.ProficiencyLevel &&
				s.YearsOfExperience == command.YearsOfExperience &&
				s.CreatedBy == command.CreatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenConflictExists_ReturnsConflictWithoutAdding()
	{
		var command = CreateValidCommand();
		var existingSkill = CandidateSkills.Create(
			command.CandidateId,
			command.Name,
			"Beginner",
			1,
			"system");

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([existingSkill]);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.Conflict, result.Status);
		Assert.Equal(
			Error.Conflict("Candidate skill with this name already exists for the candidate").Message,
			result.Error!.Message);

		_repositoryMock.Verify(r => r.AddAsync(It.IsAny<CandidateSkills>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenAddReturnsZero_ReturnsBadRequest()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<CandidateSkills>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.BadRequest, result.Status);
		Assert.Equal(Error.EntityCouldNotBeCreated("CandidateSkills").Message, result.Error!.Message);
	}

	private static AddCandidateSkillsCommand CreateValidCommand() => new()
	{
		CandidateId = 1,
		Name = "C#",
		ProficiencyLevel = "Advanced",
		YearsOfExperience = 5,
		CreatedBy = "admin"
	};

	private static bool HasProperty(object instance, string propertyName, object? expectedValue)
	{
		var property = instance.GetType().GetProperty(propertyName);
		return property is not null && Equals(property.GetValue(instance), expectedValue);
	}
}
