using Candidate.Application.Abstractions.Data;
using Candidate.Application.Features.CandidateExperience.Commands.UpdateCandidateExperiences;
using Candidate.Domain.CandidateExperience;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Candidate.Application.Tests.Features.CandidateExperience.Commands;

public class UpdateCandidateExperiencesCommandHandlerTests
{
	private readonly Mock<IGenericRepository<CandidateExperiences>> _repositoryMock;
	private readonly UpdateCandidateExperiencesCommandHandler _handler;

	public UpdateCandidateExperiencesCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<CandidateExperiences>>();
		_handler = new UpdateCandidateExperiencesCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenUpdateSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingExperience(command);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<CandidateExperiences>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.UpdateAsync(It.Is<CandidateExperiences>(e =>
				e.Company == command.Company &&
				e.Title == command.Title &&
				e.Location == command.Location &&
				e.StartDate == command.StartDate &&
				e.EndDate == command.EndDate &&
				e.IsCurrent == command.IsCurrent &&
				e.Description == command.Description &&
				e.IsActive == command.IsActive &&
				e.UpdatedBy == command.UpdatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenExperienceNotFound_ReturnsNotFound()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync((CandidateExperiences?)null!);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("CandidateExperience").Message, result.Error!.Message);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CandidateExperiences>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenUpdateReturnsZero_ReturnsNotFound()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingExperience(command);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<CandidateExperiences>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("CandidateExperiences").Message, result.Error!.Message);
	}

	private static UpdateCandidateExperiencesCommand CreateValidCommand() => new()
	{
		Id = 10,
		CandidateId = 1,
		Company = "Acme Corp",
		Title = "Senior Software Engineer",
		Location = "Hybrid",
		StartDate = new DateOnly(2024, 3, 1),
		EndDate = null,
		IsCurrent = true,
		Description = "Led platform migration",
		IsActive = true,
		UpdatedBy = "admin"
	};

	private static CandidateExperiences CreateExistingExperience(UpdateCandidateExperiencesCommand command)
	{
		var experience = CandidateExperiences.Create(
			command.CandidateId,
			"Old Corp",
			"Junior Developer",
			"On-site",
			new DateOnly(2019, 1, 1),
			new DateOnly(2021, 12, 31),
			false,
			"Maintained legacy apps",
			"creator");

		experience.Id = command.Id;
		experience.IsActive = false;

		return experience;
	}
}
