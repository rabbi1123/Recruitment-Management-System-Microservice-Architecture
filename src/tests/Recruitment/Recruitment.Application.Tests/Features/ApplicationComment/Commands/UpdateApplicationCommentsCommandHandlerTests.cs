using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Features.ApplicationComment.Commands.UpdateApplicationComments;
using Recruitment.Domain.ApplicationComment;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Recruitment.Application.Tests.Features.ApplicationComment.Commands;

public class UpdateApplicationCommentsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<ApplicationComments>> _repositoryMock;
	private readonly UpdateApplicationCommentsCommandHandler _handler;

	public UpdateApplicationCommentsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<ApplicationComments>>();
		_handler = new UpdateApplicationCommentsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenUpdateSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existing = ApplicationComments.Create(1, 2, "Old comment", "creator");
		existing.Id = command.Id;

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<ApplicationComments>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.UpdateAsync(It.Is<ApplicationComments>(c =>
				c.Id == command.Id &&
				c.Comment == command.Comment &&
				c.IsActive == command.IsActive &&
				c.UpdatedBy == command.UpdatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenCommentNotFound_ReturnsNotFound()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync((ApplicationComments?)null!);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("ApplicationComment").Message, result.Error!.Message);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<ApplicationComments>()), Times.Never);
	}

	private static UpdateApplicationCommentsCommand CreateValidCommand() => new()
	{
		Id = 10,
		Comment = "Updated comment",
		IsActive = true,
		UpdatedBy = "recruiter"
	};
}
