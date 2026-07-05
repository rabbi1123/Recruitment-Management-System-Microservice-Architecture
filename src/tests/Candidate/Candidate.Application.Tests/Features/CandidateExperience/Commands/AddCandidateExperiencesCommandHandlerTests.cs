using Candidate.Application.Abstractions.Data;
using Candidate.Application.Features.CandidateExperience.Commands.AddCandidateExperiences;
using Candidate.Domain.CandidateExperience;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Candidate.Application.Tests.Features.CandidateExperience.Commands;

public class AddCandidateExperiencesCommandHandlerTests
{
	private readonly Mock<IGenericRepository<CandidateExperiences>> _repositoryMock;
	private readonly AddCandidateExperiencesCommandHandler _handler;

	public AddCandidateExperiencesCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<CandidateExperiences>>();
		_handler = new AddCandidateExperiencesCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenAddSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<CandidateExperiences>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.AddAsync(It.Is<CandidateExperiences>(e =>
				e.CandidateId == command.CandidateId &&
				e.Company == command.Company &&
				e.Title == command.Title &&
				e.Location == command.Location &&
				e.StartDate == command.StartDate &&
				e.EndDate == command.EndDate &&
				e.IsCurrent == command.IsCurrent &&
				e.Description == command.Description &&
				e.CreatedBy == command.CreatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenAddReturnsZero_ReturnsBadRequest()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<CandidateExperiences>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.BadRequest, result.Status);
		Assert.Equal(Error.EntityCouldNotBeCreated("CandidateExperiences").Message, result.Error!.Message);
	}

	private static AddCandidateExperiencesCommand CreateValidCommand() => new()
	{
		CandidateId = 1,
		Company = "Acme Corp",
		Title = "Software Engineer",
		Location = "Remote",
		StartDate = new DateOnly(2021, 3, 1),
		EndDate = new DateOnly(2024, 2, 28),
		IsCurrent = false,
		Description = "Built microservices",
		CreatedBy = "admin"
	};
}
