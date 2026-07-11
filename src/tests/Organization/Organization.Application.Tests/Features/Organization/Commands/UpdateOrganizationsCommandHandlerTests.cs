using Organization.Application.Abstractions.Data;
using Organization.Application.Features.Organization.Commands.UpdateOrganizations;
using Organization.Domain.Organization;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Organization.Application.Tests.Features.Organization.Commands;

public class UpdateOrganizationsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Organizations>> _repositoryMock;
	private readonly UpdateOrganizationsCommandHandler _handler;

	public UpdateOrganizationsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Organizations>>();
		_handler = new UpdateOrganizationsCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenUpdateSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingOrganization(command);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<Organizations>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.UpdateAsync(It.Is<Organizations>(o =>
				o.Id == command.Id &&
				o.Name == command.Name &&
				o.Industry == command.Industry &&
				o.Website == command.Website &&
				o.Email == command.Email &&
				o.Phone == command.Phone &&
				o.Address == command.Address &&
				o.LogoUrl == command.LogoUrl &&
				o.IsActive == command.IsActive &&
				o.UpdatedBy == command.UpdatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenOrganizationNotFound_ReturnsNotFound()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync((Organizations?)null!);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("Organization").Message, result.Error!.Message);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Organizations>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WhenUpdateReturnsZero_ReturnsNotFound()
	{
		var command = CreateValidCommand();
		var existing = CreateExistingOrganization(command);

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<Organizations>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("Organizations").Message, result.Error!.Message);
	}

	private static UpdateOrganizationsCommand CreateValidCommand() => new()
	{
		Id = 10,
		Name = "Acme Corp",
		Industry = "Technology",
		Website = "https://acme.example.com",
		Email = "contact@acme.example.com",
		Phone = "555-0100",
		Address = "123 Main St",
		LogoUrl = "https://acme.example.com/logo.png",
		IsActive = true,
		UpdatedBy = "admin"
	};

	private static Organizations CreateExistingOrganization(UpdateOrganizationsCommand command)
	{
		var organization = Organizations.Create(
			"Old Name",
			"Old Industry",
			"https://old.example.com",
			"old@example.com",
			"000-0000",
			"Old Address",
			null,
			"creator");

		organization.Id = command.Id;
		organization.IsActive = false;

		return organization;
	}
}
