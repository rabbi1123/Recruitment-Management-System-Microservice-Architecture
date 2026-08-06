using Candidate.Application.Abstractions.Data;
using Candidate.Application.Features.Candidate.Commands.AddCandidates;
using Candidate.Domain.Candidate;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Candidate.Application.Tests.Features.Candidate.Commands;

public class AddCandidatesCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Candidates>> _repositoryMock;
	private readonly AddCandidatesCommandHandler _handler;

	public AddCandidatesCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Candidates>>();
		_handler = new AddCandidatesCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenNoConflictAndAddSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), true, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<Candidates>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.False(result.IsFailure);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.GetByPropertiesAsync(
				It.Is<object>(filters =>
					HasProperty(filters, "UserId", command.UserId) &&
					HasProperty(filters, "Email", command.Email)),
				true,
				true),
			Times.Once);

		_repositoryMock.Verify(
			r => r.AddAsync(It.Is<Candidates>(c =>
				c.UserId == command.UserId &&
				c.FirstName == command.FirstName &&
				c.LastName == command.LastName &&
				c.Email == command.Email &&
				c.Phone == command.Phone &&
				c.Address == command.Address &&
				c.LinkedinProfile == command.LinkedinProfile &&
				c.PortfolioUrl == command.PortfolioUrl &&
				c.Headline == command.Headline &&
				c.Summary == command.Summary &&
				c.CreatedBy == command.CreatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenConflictExists_ReturnsConflictWithoutAdding()
	{
		var command = CreateValidCommand();
		var existingCandidate = Candidates.Create(
			command.UserId,
			"Existing",
			"User",
			command.Email,
			null,
			null,
			null,
			null,
			null,
			null,
			"system");

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), true, true))
			.ReturnsAsync([existingCandidate]);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.False(result.IsSuccess);
		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.Conflict, result.Status);
		Assert.NotNull(result.Error);
		Assert.Equal(Error.Conflict("Candidates with this user id or email").Message, result.Error!.Message);

		_repositoryMock.Verify(r => r.AddAsync(It.IsAny<Candidates>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenAddReturnsZero_ReturnsBadRequest()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), true, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<Candidates>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.False(result.IsSuccess);
		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.BadRequest, result.Status);
		Assert.NotNull(result.Error);
		Assert.Equal(Error.EntityCouldNotBeCreated("Candidates").Message, result.Error!.Message);
	}

	private static AddCandidatesCommand CreateValidCommand() => new()
	{
		UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
		FirstName = "Jane",
		LastName = "Doe",
		Email = "jane.doe@example.com",
		Phone = "555-0100",
		Address = "123 Main St",
		LinkedinProfile = "https://linkedin.com/in/janedoe",
		PortfolioUrl = "https://janedoe.dev",
		Headline = "Software Engineer",
		Summary = "Experienced developer",
		CreatedBy = "admin"
	};

	private static bool HasProperty(object instance, string propertyName, object? expectedValue)
	{
		var property = instance.GetType().GetProperty(propertyName);
		return property is not null && Equals(property.GetValue(instance), expectedValue);
	}
}
