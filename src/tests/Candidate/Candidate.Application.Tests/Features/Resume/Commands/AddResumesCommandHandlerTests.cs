using Candidate.Application.Abstractions.Data;
using Candidate.Application.Features.Resume.Commands.AddResumes;
using Candidate.Domain.Resume;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Candidate.Application.Tests.Features.Resume.Commands;

public class AddResumesCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Resumes>> _repositoryMock;
	private readonly AddResumesCommandHandler _handler;

	public AddResumesCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Resumes>>();
		_handler = new AddResumesCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenAddSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<Resumes>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.AddAsync(It.Is<Resumes>(resume =>
				resume.CandidateId == command.CandidateId &&
				resume.FileName == command.FileName &&
				resume.FileUrl == command.FileUrl &&
				resume.FileFormat == command.FileFormat &&
				resume.FileSize == command.FileSize &&
				resume.IsDefault == command.IsDefault &&
				resume.CreatedBy == command.CreatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenAddReturnsZero_ReturnsBadRequest()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<Resumes>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.BadRequest, result.Status);
		Assert.Equal(Error.EntityCouldNotBeCreated("Resumes").Message, result.Error!.Message);
	}

	private static AddResumesCommand CreateValidCommand() => new()
	{
		CandidateId = 1,
		FileName = "jane-doe-resume.pdf",
		FileUrl = "https://files.example.com/resumes/jane-doe.pdf",
		FileFormat = "pdf",
		FileSize = 204800,
		IsDefault = true,
		CreatedBy = "admin"
	};
}
