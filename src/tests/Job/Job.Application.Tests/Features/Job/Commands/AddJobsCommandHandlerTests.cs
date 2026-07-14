using Job.Application.Abstractions.Data;
using Job.Application.Features.Job.Commands.AddJobs;
using Job.Domain.Job;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Job.Application.Tests.Features.Job.Commands;

public class AddJobsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Jobs>> _repositoryMock;
	private readonly AddJobsCommandHandler _handler;

	public AddJobsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Jobs>>();
		_handler = new AddJobsCommandHandler(_repositoryMock.Object);
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
