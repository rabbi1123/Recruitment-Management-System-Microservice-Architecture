using Job.Application.Abstractions.Data;
using Job.Application.Features.Job.Commands.UpdateJobs;
using Job.Domain.Job;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Job.Application.Tests.Features.Job.Commands;

public class UpdateJobsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Jobs>> _repositoryMock;
	private readonly UpdateJobsCommandHandler _handler;

	public UpdateJobsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Jobs>>();
		_handler = new UpdateJobsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenUpdateSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingJob(command);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<Jobs>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.UpdateAsync(It.Is<Jobs>(j =>
				j.Id == command.Id &&
				j.OrganizationId == command.OrganizationId &&
				j.RecruiterId == command.RecruiterId &&
				j.Title == command.Title &&
				j.Department == command.Department &&
				j.EmploymentType == command.EmploymentType &&
				j.Location == command.Location &&
				j.IsRemote == command.IsRemote &&
				j.Status == command.Status &&
				j.IsActive == command.IsActive &&
				j.UpdatedBy == command.UpdatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenJobNotFound_ReturnsNotFound()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync((Jobs?)null!);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("Job").Message, result.Error!.Message);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Jobs>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenUpdateReturnsZero_ReturnsNotFound()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingJob(command);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<Jobs>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("Jobs").Message, result.Error!.Message);
	}

	private static UpdateJobsCommand CreateValidCommand() => new()
	{
		Id = 10,
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
		Status = "Published",
		IsActive = true,
		UpdatedBy = "admin"
	};

	private static Jobs CreateExistingJob(UpdateJobsCommand command)
	{
		var job = Jobs.Create(
			command.OrganizationId,
			command.RecruiterId,
			"Old Title",
			"Old Department",
			"PartTime",
			"Old Location",
			false,
			1,
			2,
			50000,
			60000,
			"USD",
			"Old description",
			"Draft",
			"creator");

		job.Id = command.Id;
		job.IsActive = false;

		return job;
	}
}
