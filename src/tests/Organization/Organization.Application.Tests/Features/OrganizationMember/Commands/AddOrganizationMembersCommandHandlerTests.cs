using Organization.Application.Abstractions.Data;
using Organization.Application.Features.OrganizationMember.Commands.AddOrganizationMembers;
using Organization.Domain.OrganizationMember;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Organization.Application.Tests.Features.OrganizationMember.Commands;

public class AddOrganizationMembersCommandHandlerTests
{
	private readonly Mock<IGenericRepository<OrganizationMembers>> _repositoryMock;
	private readonly AddOrganizationMembersCommandHandler _handler;

	public AddOrganizationMembersCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<OrganizationMembers>>();
		_handler = new AddOrganizationMembersCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenNoConflictAndAddSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<OrganizationMembers>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.GetByPropertiesAsync(
				It.Is<object>(filters =>
					HasProperty(filters, "OrganizationId", command.OrganizationId) &&
					HasProperty(filters, "UserId", command.UserId)),
				false,
				true),
			Times.Once);

		_repositoryMock.Verify(
			r => r.AddAsync(It.Is<OrganizationMembers>(m =>
				m.OrganizationId == command.OrganizationId &&
				m.UserId == command.UserId &&
				m.FullName == command.FullName &&
				m.Email == command.Email &&
				m.Phone == command.Phone &&
				m.Role == command.Role &&
				m.CreatedBy == command.CreatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenConflictExists_ReturnsConflictWithoutAdding()
	{
		var command = CreateValidCommand();
		var existingMember = OrganizationMembers.Create(
			command.OrganizationId,
			command.UserId,
			"Existing User",
			command.Email,
			command.Phone,
			command.Role,
			"system");

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([existingMember]);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.Conflict, result.Status);
		Assert.Equal(
			Error.Conflict("Organization member with this user already exists for the organization").Message,
			result.Error!.Message);

		_repositoryMock.Verify(r => r.AddAsync(It.IsAny<OrganizationMembers>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenAddReturnsZero_ReturnsBadRequest()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByPropertiesAsync(It.IsAny<object>(), false, true))
			.ReturnsAsync([]);

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<OrganizationMembers>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.BadRequest, result.Status);
		Assert.Equal(Error.EntityCouldNotBeCreated("OrganizationMembers").Message, result.Error!.Message);
	}

	private static AddOrganizationMembersCommand CreateValidCommand() => new()
	{
		OrganizationId = 1,
		UserId = 42,
		FullName = "Jane Doe",
		Email = "jane.doe@example.com",
		Phone = "555-0100",
		Role = "Recruiter",
		CreatedBy = "admin"
	};

	private static bool HasProperty(object instance, string propertyName, object? expectedValue)
	{
		var property = instance.GetType().GetProperty(propertyName);
		return property is not null && Equals(property.GetValue(instance), expectedValue);
	}
}
