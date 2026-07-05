using Candidate.Application.Abstractions.Data;
using Candidate.Application.Features.CandidateEducation.Commands.AddCandidateEducations;
using Candidate.Domain.CandidateEducation;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Candidate.Application.Tests.Features.CandidateEducation.Commands;

public class AddCandidateEducationsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<CandidateEducations>> _repositoryMock;
	private readonly AddCandidateEducationsCommandHandler _handler;

	public AddCandidateEducationsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<CandidateEducations>>();
		_handler = new AddCandidateEducationsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenAddSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<CandidateEducations>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.AddAsync(It.Is<CandidateEducations>(e =>
				e.CandidateId == command.CandidateId &&
				e.Institution == command.Institution &&
				e.Degree == command.Degree &&
				e.FieldOfStudy == command.FieldOfStudy &&
				e.StartDate == command.StartDate &&
				e.EndDate == command.EndDate &&
				e.Grade == command.Grade &&
				e.CreatedBy == command.CreatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenAddReturnsZero_ReturnsBadRequest()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<CandidateEducations>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.BadRequest, result.Status);
		Assert.Equal(Error.EntityCouldNotBeCreated("CandidateEducations").Message, result.Error!.Message);
	}

	private static AddCandidateEducationsCommand CreateValidCommand() => new()
	{
		CandidateId = 1,
		Institution = "State University",
		Degree = "BSc",
		FieldOfStudy = "Computer Science",
		StartDate = new DateOnly(2018, 9, 1),
		EndDate = new DateOnly(2022, 6, 30),
		Grade = "3.8",
		CreatedBy = "admin"
	};
}
