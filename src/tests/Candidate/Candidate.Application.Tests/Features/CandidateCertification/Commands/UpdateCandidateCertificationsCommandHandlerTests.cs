using Candidate.Application.Abstractions.Data;
using Candidate.Application.Features.CandidateCertification.Commands.UpdateCandidateCertifications;
using Candidate.Domain.CandidateCertification;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Candidate.Application.Tests.Features.CandidateCertification.Commands;

public class UpdateCandidateCertificationsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<CandidateCertifications>> _repositoryMock;
	private readonly UpdateCandidateCertificationsCommandHandler _handler;

	public UpdateCandidateCertificationsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<CandidateCertifications>>();
		_handler = new UpdateCandidateCertificationsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenUpdateSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingCertification(command);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<CandidateCertifications>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.UpdateAsync(It.Is<CandidateCertifications>(c =>
				c.Name == command.Name &&
				c.IssuingOrg == command.IssuingOrg &&
				c.IssueDate == command.IssueDate &&
				c.ExpiryDate == command.ExpiryDate &&
				c.CredentialId == command.CredentialId &&
				c.CredentialUrl == command.CredentialUrl &&
				c.IsActive == command.IsActive &&
				c.UpdatedBy == command.UpdatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenCertificationNotFound_ReturnsNotFound()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync((CandidateCertifications?)null!);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("CandidateCertification").Message, result.Error!.Message);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CandidateCertifications>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenUpdateReturnsZero_ReturnsNotFound()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingCertification(command);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<CandidateCertifications>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("CandidateCertifications").Message, result.Error!.Message);
	}

	private static UpdateCandidateCertificationsCommand CreateValidCommand() => new()
	{
		Id = 10,
		CandidateId = 1,
		Name = "AWS Solutions Architect Professional",
		IssuingOrg = "Amazon",
		IssueDate = new DateOnly(2024, 6, 1),
		ExpiryDate = new DateOnly(2027, 6, 1),
		CredentialId = "ABC-456",
		CredentialUrl = "https://aws.amazon.com/verify/pro",
		IsActive = true,
		UpdatedBy = "admin"
	};

	private static CandidateCertifications CreateExistingCertification(UpdateCandidateCertificationsCommand command)
	{
		var certification = CandidateCertifications.Create(
			command.CandidateId,
			"Old Certification",
			"Old Org",
			new DateOnly(2020, 1, 1),
			new DateOnly(2023, 1, 1),
			"OLD-123",
			null,
			"creator");

		certification.Id = command.Id;
		certification.IsActive = false;

		return certification;
	}
}
