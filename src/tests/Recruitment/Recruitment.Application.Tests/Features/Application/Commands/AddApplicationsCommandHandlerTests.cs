using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Common.Data;
using Recruitment.Application.Features.Application.Commands.AddApplications;
using Recruitment.Domain.Application;
using Common.Platform.Domain.Abstractions;
using Messaging.Contracts.Events.Email;
using Messaging.Contracts.Events.Push;
using Messaging.Contracts.Events.Sms;
using Messaging.RabbitMq;
using Moq;
using Xunit;

namespace Recruitment.Application.Tests.Features.Application.Commands;

public class AddApplicationsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Applications>> _repositoryMock;
	private readonly PostCommitActions _postCommit;
	private readonly Mock<IIntegrationEventPublisher> _publisherMock;
	private readonly AddApplicationsCommandHandler _handler;

	public AddApplicationsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Applications>>();
		_postCommit = new PostCommitActions();
		_publisherMock = new Mock<IIntegrationEventPublisher>();
		_handler = new AddApplicationsCommandHandler(
			_repositoryMock.Object,
			_postCommit,
			_publisherMock.Object);
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
		Assert.Equal(1, result.Data.Id);

		await _postCommit.RunAsync(CancellationToken.None);
		_publisherMock.Verify(
			p => p.PublishAsync(
				It.Is<ApplicationSubmittedEmailEvent>(e =>
					e.ApplicationId == 1 &&
					e.JobId == command.JobId &&
					e.CandidateId == command.CandidateId &&
					e.OrganizationId == command.OrganizationId),
				It.IsAny<CancellationToken>()),
			Times.Once);
		_publisherMock.Verify(
			p => p.PublishAsync(
				It.Is<ApplicationSubmittedSmsEvent>(e =>
					e.ApplicationId == 1 &&
					e.JobId == command.JobId &&
					e.CandidateId == command.CandidateId &&
					e.OrganizationId == command.OrganizationId),
				It.IsAny<CancellationToken>()),
			Times.Once);
		_publisherMock.Verify(
			p => p.PublishAsync(
				It.Is<ApplicationSubmittedPushEvent>(e =>
					e.ApplicationId == 1 &&
					e.JobId == command.JobId &&
					e.CandidateId == command.CandidateId &&
					e.OrganizationId == command.OrganizationId),
				It.IsAny<CancellationToken>()),
			Times.Once);

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

		await _postCommit.RunAsync(CancellationToken.None);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<ApplicationSubmittedEmailEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<ApplicationSubmittedSmsEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<ApplicationSubmittedPushEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
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

		await _postCommit.RunAsync(CancellationToken.None);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<ApplicationSubmittedEmailEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<ApplicationSubmittedSmsEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<ApplicationSubmittedPushEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
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
