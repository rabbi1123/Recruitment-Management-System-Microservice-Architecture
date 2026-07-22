using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Features.Offer.Commands.UpdateOffers;
using Recruitment.Domain.Offer;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Recruitment.Application.Tests.Features.Offer.Commands;

public class UpdateOffersCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Offers>> _repositoryMock;
	private readonly UpdateOffersCommandHandler _handler;

	public UpdateOffersCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Offers>>();
		_handler = new UpdateOffersCommandHandler(_repositoryMock.Object);
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
