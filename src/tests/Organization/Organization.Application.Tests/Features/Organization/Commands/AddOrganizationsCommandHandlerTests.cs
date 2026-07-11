using Organization.Application.Abstractions.Data;
using Organization.Application.Features.Organization.Commands.AddOrganizations;
using Organization.Domain.Organization;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Organization.Application.Tests.Features.Organization.Commands;

public class AddOrganizationsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Organizations>> _repositoryMock;
	private readonly AddOrganizationsCommandHandler _handler;

	public AddOrganizationsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Organizations>>();
		_handler = new AddOrganizationsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenAddSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<Organizations>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.AddAsync(It.Is<Organizations>(o =>
				o.Name == command.Name &&
				o.Industry == command.Industry &&
				o.Website == command.Website &&
				o.Email == command.Email &&
				o.Phone == command.Phone &&
				o.Address == command.Address &&
				o.LogoUrl == command.LogoUrl &&
				o.CreatedBy == command.CreatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenAddReturnsZero_ReturnsBadRequest()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<Organizations>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.BadRequest, result.Status);
		Assert.Equal(Error.EntityCouldNotBeCreated("Organizations").Message, result.Error!.Message);
	}

	private static AddOrganizationsCommand CreateValidCommand() => new()
	{
		Name = "Acme Corp",
		Industry = "Technology",
		Website = "https://acme.example.com",
		Email = "contact@acme.example.com",
		Phone = "555-0100",
		Address = "123 Main St",
		LogoUrl = "https://acme.example.com/logo.png",
		CreatedBy = "admin"
	};
}
