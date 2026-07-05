using Candidate.Application.Abstractions.Data;
using Candidate.Application.Features.CandidateEducation.Commands.UpdateCandidateEducations;
using Candidate.Domain.CandidateEducation;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Candidate.Application.Tests.Features.CandidateEducation.Commands;

public class UpdateCandidateEducationsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<CandidateEducations>> _repositoryMock;
	private readonly UpdateCandidateEducationsCommandHandler _handler;

	public UpdateCandidateEducationsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<CandidateEducations>>();
		_handler = new UpdateCandidateEducationsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenUpdateSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingEducation(command);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<CandidateEducations>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.UpdateAsync(It.Is<CandidateEducations>(e =>
				e.Institution == command.Institution &&
				e.Degree == command.Degree &&
				e.FieldOfStudy == command.FieldOfStudy &&
				e.StartDate == command.StartDate &&
				e.EndDate == command.EndDate &&
				e.Grade == command.Grade &&
				e.IsActive == command.IsActive &&
				e.UpdatedBy == command.UpdatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenEducationNotFound_ReturnsNotFound()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync((CandidateEducations?)null!);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("CandidateEducation").Message, result.Error!.Message);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CandidateEducations>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenUpdateReturnsZero_ReturnsNotFound()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingEducation(command);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<CandidateEducations>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("CandidateEducations").Message, result.Error!.Message);
	}

	private static UpdateCandidateEducationsCommand CreateValidCommand() => new()
	{
		Id = 10,
		CandidateId = 1,
		Institution = "State University",
		Degree = "MSc",
		FieldOfStudy = "Software Engineering",
		StartDate = new DateOnly(2022, 9, 1),
		EndDate = new DateOnly(2024, 6, 30),
		Grade = "4.0",
		IsActive = true,
		UpdatedBy = "admin"
	};

	private static CandidateEducations CreateExistingEducation(UpdateCandidateEducationsCommand command)
	{
		var education = CandidateEducations.Create(
			command.CandidateId,
			"Old University",
			"BSc",
			"IT",
			new DateOnly(2014, 9, 1),
			new DateOnly(2018, 6, 30),
			"3.5",
			"creator");

		education.Id = command.Id;
		education.IsActive = false;

		return education;
	}
}
