using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Features.Application.Commands.AddApplications;
using Recruitment.Domain.Application;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Recruitment.Application.Tests.Features.Application.Commands;

public class AddApplicationsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Applications>> _repositoryMock;
	private readonly AddApplicationsCommandHandler _handler;

	public AddApplicationsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Applications>>();
		_handler = new AddApplicationsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenAddSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<Applications>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.AddAsync(It.Is<Applications>(a =>
				a.JobId == command.JobId &&
				a.CandidateId == command.CandidateId &&
				a.OrganizationId == command.OrganizationId &&
				a.RecruiterId == command.RecruiterId &&
				a.CoverLetter == command.CoverLetter &&
				a.CurrentStage == "Applied" &&
				a.Status == "Active" &&
				a.CreatedBy == command.CreatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenDuplicateJobAndCandidate_ReturnsConflict()
	{
		var command = CreateValidCommand();
		var existing = Applications.Create(command.JobId, command.CandidateId, command.OrganizationId, null, null, "creator");
		existing.Id = 1;

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([existing]);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.Conflict, result.Status);
		_repositoryMock.Verify(r => r.AddAsync(It.IsAny<Applications>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenAddReturnsZero_ReturnsBadRequest()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<Applications>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.BadRequest, result.Status);
		Assert.Equal(Error.EntityCouldNotBeCreated("Applications").Message, result.Error!.Message);
	}

	private static AddApplicationsCommand CreateValidCommand() => new()
	{
		JobId = 1,
		CandidateId = 2,
		OrganizationId = 3,
		RecruiterId = 4,
		CoverLetter = "I am interested in this role.",
		CreatedBy = "recruiter"
	};
}
