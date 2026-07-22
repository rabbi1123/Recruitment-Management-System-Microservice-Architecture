using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Features.ApplicationComment.Commands.AddApplicationComments;
using Recruitment.Domain.ApplicationComment;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Recruitment.Application.Tests.Features.ApplicationComment.Commands;

public class AddApplicationCommentsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<ApplicationComments>> _repositoryMock;
	private readonly AddApplicationCommentsCommandHandler _handler;

	public AddApplicationCommentsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<ApplicationComments>>();
		_handler = new AddApplicationCommentsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenAddSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<ApplicationComments>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.AddAsync(It.Is<ApplicationComments>(c =>
				c.ApplicationId == command.ApplicationId &&
				c.AuthorId == command.AuthorId &&
				c.Comment == command.Comment &&
				c.CreatedBy == command.CreatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenAddReturnsZero_ReturnsBadRequest()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<ApplicationComments>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.BadRequest, result.Status);
		Assert.Equal(Error.EntityCouldNotBeCreated("ApplicationComments").Message, result.Error!.Message);
	}

	private static AddApplicationCommentsCommand CreateValidCommand() => new()
	{
		ApplicationId = 1,
		AuthorId = 2,
		Comment = "Strong candidate, recommend for interview.",
		CreatedBy = "recruiter"
	};
}
