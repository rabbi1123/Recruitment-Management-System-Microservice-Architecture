using Organization.Application.Abstractions.Data;
using Organization.Application.Features.OrganizationMember.Commands.UpdateOrganizationMembers;
using Organization.Domain.OrganizationMember;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Organization.Application.Tests.Features.OrganizationMember.Commands;

public class UpdateOrganizationMembersCommandHandlerTests
{
	private readonly Mock<IGenericRepository<OrganizationMembers>> _repositoryMock;
	private readonly UpdateOrganizationMembersCommandHandler _handler;

	public UpdateOrganizationMembersCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<OrganizationMembers>>();
		_handler = new UpdateOrganizationMembersCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenNoConflictAndUpdateSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingMember(command);

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<OrganizationMembers>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.UpdateAsync(It.Is<OrganizationMembers>(m =>
				m.FullName == command.FullName &&
				m.Email == command.Email &&
				m.Phone == command.Phone &&
				m.Role == command.Role &&
				m.IsActive == command.IsActive &&
				m.UpdatedBy == command.UpdatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenOnlyMatchingMemberIsSameId_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingMember(command);

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([existing]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<OrganizationMembers>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<OrganizationMembers>()), Times.Once);
	}

	[Fact]
	public async Task Handle_WhenConflictExistsWithDifferentId_ReturnsConflictWithoutUpdating()
	{
		var command = CreateValidCommand();
		var conflictingMember = OrganizationMembers.Create(
			command.OrganizationId,
			command.UserId,
			"Other User",
			command.Email,
			command.Phone,
			"OrganizationAdmin",
			"system");
		conflictingMember.Id = command.Id + 1;

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([conflictingMember]);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.Conflict, result.Status);
		Assert.Equal(
			Error.Conflict("Organization member with this user already exists for the organization").Message,
			result.Error!.Message);

		_repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<OrganizationMembers>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenMemberNotFound_ReturnsNotFound()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync((OrganizationMembers?)null!);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("OrganizationMember").Message, result.Error!.Message);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<OrganizationMembers>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenUpdateReturnsZero_ReturnsNotFound()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingMember(command);

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<OrganizationMembers>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("OrganizationMembers").Message, result.Error!.Message);
	}

	private static UpdateOrganizationMembersCommand CreateValidCommand() => new()
	{
		Id = 10,
		OrganizationId = 1,
		UserId = 42,
		FullName = "Jane Doe",
		Email = "jane.doe@example.com",
		Phone = "555-0100",
		Role = "Recruiter",
		IsActive = true,
		UpdatedBy = "admin"
	};

	private static OrganizationMembers CreateExistingMember(UpdateOrganizationMembersCommand command)
	{
		var member = OrganizationMembers.Create(
			command.OrganizationId,
			command.UserId,
			"Old Name",
			"old@example.com",
			"000-0000",
			"OrganizationAdmin",
			"creator");

		member.Id = command.Id;
		member.IsActive = false;

		return member;
	}
}
