using Job.Application.Abstractions.Data;
using Job.Application.Common.Data;
using Job.Application.Features.Job.Commands.AddJobs;
using Job.Domain.Job;
using Common.Platform.Domain.Abstractions;
using Messaging.Contracts.Events.Email;
using Messaging.Contracts.Events.Push;
using Messaging.Contracts.Events.Sms;
using Messaging.RabbitMq;
using Moq;
using Xunit;

namespace Job.Application.Tests.Features.Job.Commands;

public class AddJobsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Jobs>> _repositoryMock;
	private readonly PostCommitActions _postCommit;
	private readonly Mock<IIntegrationEventPublisher> _publisherMock;
	private readonly AddJobsCommandHandler _handler;

	public AddJobsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Jobs>>();
		_postCommit = new PostCommitActions();
		_publisherMock = new Mock<IIntegrationEventPublisher>();
		_handler = new AddJobsCommandHandler(
			_repositoryMock.Object,
			_postCommit,
			_publisherMock.Object);
	}

	[Fact]
	public async Task Handle_WhenAddSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<Jobs>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);
		Assert.Equal(1, result.Data.Id);

		await _postCommit.RunAsync(CancellationToken.None);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<JobPublishedEmailEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<JobPublishedSmsEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<JobPublishedPushEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);

		_repositoryMock.Verify(
			r => r.AddAsync(It.Is<Jobs>(j =>
				j.OrganizationId == command.OrganizationId &&
				j.RecruiterId == command.RecruiterId &&
				j.Title == command.Title &&
				j.Department == command.Department &&
				j.EmploymentType == command.EmploymentType &&
				j.Location == command.Location &&
				j.IsRemote == command.IsRemote &&
				j.ExperienceMin == command.ExperienceMin &&
				j.ExperienceMax == command.ExperienceMax &&
				j.SalaryMin == command.SalaryMin &&
				j.SalaryMax == command.SalaryMax &&
				j.Currency == command.Currency &&
				j.Description == command.Description &&
				j.Status == command.Status &&
				j.CreatedBy == command.CreatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenAddReturnsZero_ReturnsBadRequest()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<Jobs>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.BadRequest, result.Status);
		Assert.Equal(Error.EntityCouldNotBeCreated("Jobs").Message, result.Error!.Message);

		await _postCommit.RunAsync(CancellationToken.None);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<JobPublishedEmailEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<JobPublishedSmsEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<JobPublishedPushEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
	}

	[Fact]
	public async Task Handle_WhenCreatedAsPublished_SchedulesJobPublished()
	{
		var command = CreateValidCommand();
		command.Status = "Published";

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<Jobs>()))
			.ReturnsAsync(42);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.Equal(42, result.Data!.Id);

		await _postCommit.RunAsync(CancellationToken.None);
		_publisherMock.Verify(
			p => p.PublishAsync(
				It.Is<JobPublishedEmailEvent>(e =>
					e.JobId == 42 &&
					e.OrganizationId == command.OrganizationId &&
					e.Title == command.Title),
				It.IsAny<CancellationToken>()),
			Times.Once);
		_publisherMock.Verify(
			p => p.PublishAsync(
				It.Is<JobPublishedSmsEvent>(e =>
					e.JobId == 42 &&
					e.OrganizationId == command.OrganizationId &&
					e.Title == command.Title),
				It.IsAny<CancellationToken>()),
			Times.Once);
		_publisherMock.Verify(
			p => p.PublishAsync(
				It.Is<JobPublishedPushEvent>(e =>
					e.JobId == 42 &&
					e.OrganizationId == command.OrganizationId &&
					e.Title == command.Title),
				It.IsAny<CancellationToken>()),
			Times.Once);
	}

	private static AddJobsCommand CreateValidCommand() => new()
	{
		OrganizationId = 1,
		RecruiterId = 2,
		Title = "Senior Software Engineer",
		Department = "Engineering",
		EmploymentType = "FullTime",
		Location = "Dhaka",
		IsRemote = true,
		ExperienceMin = 3,
		ExperienceMax = 7,
		SalaryMin = 80000,
		SalaryMax = 120000,
		Currency = "USD",
		Description = "Build recruitment services",
		Status = "Draft",
		CreatedBy = "admin"
	};
}
