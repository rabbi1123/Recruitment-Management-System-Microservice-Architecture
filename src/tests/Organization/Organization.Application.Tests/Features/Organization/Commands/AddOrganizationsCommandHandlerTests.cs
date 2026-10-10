using Organization.Application.Abstractions.Data;
using Organization.Application.Common.Data;
using Organization.Application.Features.Organization.Commands.AddOrganizations;
using Organization.Domain.Organization;
using Common.Platform.Domain.Abstractions;
using Messaging.Contracts.Events.Email;
using Messaging.Contracts.Events.Push;
using Messaging.Contracts.Events.Sms;
using Messaging.RabbitMq;
using Moq;
using Xunit;

namespace Organization.Application.Tests.Features.Organization.Commands;

public class AddOrganizationsCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Organizations>> _repositoryMock;
	private readonly PostCommitActions _postCommit;
	private readonly Mock<IIntegrationEventPublisher> _publisherMock;
	private readonly AddOrganizationsCommandHandler _handler;

	public AddOrganizationsCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Organizations>>();
		_postCommit = new PostCommitActions();
		_publisherMock = new Mock<IIntegrationEventPublisher>();
		_handler = new AddOrganizationsCommandHandler(
			_repositoryMock.Object,
			_postCommit,
			_publisherMock.Object);
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
		Assert.Equal(1, result.Data.Id);

		await _postCommit.RunAsync(CancellationToken.None);
		_publisherMock.Verify(
			p => p.PublishAsync(
				It.Is<OrganizationCreatedEmailEvent>(e => e.OrganizationId == 1 && e.Name == command.Name),
				It.IsAny<CancellationToken>()),
			Times.Once);
		_publisherMock.Verify(
			p => p.PublishAsync(
				It.Is<OrganizationCreatedSmsEvent>(e => e.OrganizationId == 1 && e.Name == command.Name),
				It.IsAny<CancellationToken>()),
			Times.Once);
		_publisherMock.Verify(
			p => p.PublishAsync(
				It.Is<OrganizationCreatedPushEvent>(e => e.OrganizationId == 1 && e.Name == command.Name),
				It.IsAny<CancellationToken>()),
			Times.Once);

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

		await _postCommit.RunAsync(CancellationToken.None);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<OrganizationCreatedEmailEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<OrganizationCreatedSmsEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<OrganizationCreatedPushEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
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
