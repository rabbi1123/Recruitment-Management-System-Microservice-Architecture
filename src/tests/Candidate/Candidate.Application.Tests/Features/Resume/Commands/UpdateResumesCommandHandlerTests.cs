using Candidate.Application.Abstractions.Data;
using Candidate.Application.Features.Resume.Commands.UpdateResumes;
using Candidate.Domain.Resume;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Candidate.Application.Tests.Features.Resume.Commands;

public class UpdateResumesCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Resumes>> _repositoryMock;
	private readonly UpdateResumesCommandHandler _handler;

	public UpdateResumesCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Resumes>>();
		_handler = new UpdateResumesCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenUpdateSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingResume(command);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<Resumes>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.UpdateAsync(It.Is<Resumes>(resume =>
				resume.FileName == command.FileName &&
				resume.FileUrl == command.FileUrl &&
				resume.FileFormat == command.FileFormat &&
				resume.FileSize == command.FileSize &&
				resume.IsDefault == command.IsDefault &&
				resume.IsActive == command.IsActive &&
				resume.UpdatedBy == command.UpdatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenResumeNotFound_ReturnsNotFound()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync((Resumes?)null!);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("Resume").Message, result.Error!.Message);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Resumes>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenUpdateReturnsZero_ReturnsNotFound()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingResume(command);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<Resumes>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("Resumes").Message, result.Error!.Message);
	}

	private static UpdateResumesCommand CreateValidCommand() => new()
	{
		Id = 10,
		CandidateId = 1,
		FileName = "jane-doe-resume-v2.pdf",
		FileUrl = "https://files.example.com/resumes/jane-doe-v2.pdf",
		FileFormat = "pdf",
		FileSize = 256000,
		IsDefault = true,
		IsActive = true,
		UpdatedBy = "admin"
	};

	private static Resumes CreateExistingResume(UpdateResumesCommand command)
	{
		var resume = Resumes.Create(
			command.CandidateId,
			"old-resume.pdf",
			"https://files.example.com/resumes/old.pdf",
			"pdf",
			102400,
			false,
			"creator");

		resume.Id = command.Id;
		resume.IsActive = false;

		return resume;
	}
}
