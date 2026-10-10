using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Common.Data;
using Recruitment.Application.Features.Offer.Commands.UpdateOffers;
using Recruitment.Domain.Offer;
using Common.Platform.Domain.Abstractions;
using Messaging.Contracts.Events.Email;
using Messaging.Contracts.Events.Push;
using Messaging.Contracts.Events.Sms;
using Messaging.RabbitMq;
using Moq;
using Xunit;

namespace Recruitment.Application.Tests.Features.Offer.Commands;

public class UpdateOffersCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Offers>> _repositoryMock;
	private readonly PostCommitActions _postCommit;
	private readonly Mock<IIntegrationEventPublisher> _publisherMock;
	private readonly UpdateOffersCommandHandler _handler;

	public UpdateOffersCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Offers>>();
		_postCommit = new PostCommitActions();
		_publisherMock = new Mock<IIntegrationEventPublisher>();
		_handler = new UpdateOffersCommandHandler(
			_repositoryMock.Object,
			_postCommit,
			_publisherMock.Object);
	}

	[Fact]
	public async Task Handle_WhenUpdateSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();
		var existing = Offers.Create(1, 2, 3, 4, "Developer", 100000m, "USD", null, null, null, "creator");
		existing.Id = command.Id;

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<Offers>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		await _postCommit.RunAsync(CancellationToken.None);
		_publisherMock.Verify(
			p => p.PublishAsync(
				It.Is<OfferSentEmailEvent>(e =>
					e.OfferId == command.Id &&
					e.ApplicationId == 1 &&
					e.CandidateId == 2 &&
					e.JobId == 3 &&
					e.OrganizationId == 4),
				It.IsAny<CancellationToken>()),
			Times.Once);
		_publisherMock.Verify(
			p => p.PublishAsync(
				It.Is<OfferSentSmsEvent>(e =>
					e.OfferId == command.Id &&
					e.ApplicationId == 1 &&
					e.CandidateId == 2 &&
					e.JobId == 3 &&
					e.OrganizationId == 4),
				It.IsAny<CancellationToken>()),
			Times.Once);
		_publisherMock.Verify(
			p => p.PublishAsync(
				It.Is<OfferSentPushEvent>(e =>
					e.OfferId == command.Id &&
					e.ApplicationId == 1 &&
					e.CandidateId == 2 &&
					e.JobId == 3 &&
					e.OrganizationId == 4),
				It.IsAny<CancellationToken>()),
			Times.Once);

		_repositoryMock.Verify(
			r => r.UpdateAsync(It.Is<Offers>(o =>
				o.Id == command.Id &&
				o.Position == command.Position &&
				o.Salary == command.Salary &&
				o.Currency == command.Currency &&
				o.Status == command.Status &&
				o.IsActive == command.IsActive &&
				o.UpdatedBy == command.UpdatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenOfferNotFound_ReturnsNotFound()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync((Offers?)null!);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.NotFound, result.Status);
		Assert.Equal(Error.NotFound("Offer").Message, result.Error!.Message);
		_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Offers>()), Times.Never);

		await _postCommit.RunAsync(CancellationToken.None);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<OfferSentEmailEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<OfferSentSmsEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<OfferSentPushEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
	}

	[Fact]
	public async Task Handle_WhenAlreadySent_DoesNotPublishAgain()
	{
		var command = CreateValidCommand();
		var existing = Offers.Create(1, 2, 3, 4, "Developer", 100000m, "USD", null, null, null, "creator");
		existing.Id = command.Id;
		existing.Status = "Sent";

		_repositoryMock
			.Setup(r => r.GetByIdAsync(command.Id))
			.ReturnsAsync(existing);

		_repositoryMock
			.Setup(r => r.UpdateAsync(It.IsAny<Offers>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		await _postCommit.RunAsync(CancellationToken.None);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<OfferSentEmailEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<OfferSentSmsEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
		_publisherMock.Verify(
			p => p.PublishAsync(It.IsAny<OfferSentPushEvent>(), It.IsAny<CancellationToken>()),
			Times.Never);
	}

	private static UpdateOffersCommand CreateValidCommand() => new()
	{
		Id = 10,
		Position = "Senior Developer",
		Salary = 130000m,
		Currency = "USD",
		JoiningDate = new DateTime(2026, 9, 1),
		Benefits = "Health, dental, vision",
		ExpirationDate = new DateTime(2026, 8, 15),
		Status = "Sent",
		SentDate = DateTime.UtcNow,
		RespondedDate = null,
		IsActive = true,
		UpdatedBy = "recruiter"
	};
}
