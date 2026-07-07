using Candidate.Application.Abstractions.Data;
using Candidate.Application.Features.CandidateCertification.Commands.AddCandidateCertifications;
using Candidate.Domain.CandidateCertification;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Candidate.Application.Tests.Features.CandidateCertification.Commands;

public class AddCandidateCertificationsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<CandidateCertifications>> _repositoryMock;
	private readonly AddCandidateCertificationsCommandHandler _handler;

	public AddCandidateCertificationsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<CandidateCertifications>>();
		_handler = new AddCandidateCertificationsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenAddSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<CandidateCertifications>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.AddAsync(It.Is<CandidateCertifications>(c =>
				c.CandidateId == command.CandidateId &&
				c.Name == command.Name &&
				c.IssuingOrg == command.IssuingOrg &&
				c.IssueDate == command.IssueDate &&
				c.ExpiryDate == command.ExpiryDate &&
				c.CredentialId == command.CredentialId &&
				c.CredentialUrl == command.CredentialUrl &&
				c.CreatedBy == command.CreatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenAddReturnsZero_ReturnsBadRequest()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<CandidateCertifications>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.BadRequest, result.Status);
		Assert.Equal(Error.EntityCouldNotBeCreated("CandidateCertifications").Message, result.Error!.Message);
	}

	private static AddCandidateCertificationsCommand CreateValidCommand() => new()
	{
		CandidateId = 1,
		Name = "AWS Solutions Architect",
		IssuingOrg = "Amazon",
		IssueDate = new DateOnly(2024, 1, 15),
		ExpiryDate = new DateOnly(2027, 1, 15),
		CredentialId = "ABC-123",
		CredentialUrl = "https://aws.amazon.com/verify",
		CreatedBy = "admin"
	};
}
