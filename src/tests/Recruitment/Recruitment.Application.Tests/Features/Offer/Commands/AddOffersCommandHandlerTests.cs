using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Features.Offer.Commands.AddOffers;
using Recruitment.Domain.Offer;
using Common.Platform.Domain.Abstractions;
using Moq;
using Xunit;

namespace Recruitment.Application.Tests.Features.Offer.Commands;

public class AddOffersCommandHandlerTests
{
	private readonly Mock<IGenericRepository<Offers>> _repositoryMock;
	private readonly AddOffersCommandHandler _handler;

	public AddOffersCommandHandlerTests()
	{
		_repositoryMock = new Mock<IGenericRepository<Offers>>();
		_handler = new AddOffersCommandHandler(_repositoryMock.Object);
	}

	[Fact]
	public async Task Handle_WhenAddSucceeds_ReturnsSuccess()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<Offers>()))
			.ReturnsAsync(1);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.True(result.Data!.IsSuccess);

		_repositoryMock.Verify(
			r => r.AddAsync(It.Is<Offers>(o =>
				o.ApplicationId == command.ApplicationId &&
				o.CandidateId == command.CandidateId &&
				o.JobId == command.JobId &&
				o.OrganizationId == command.OrganizationId &&
				o.Position == command.Position &&
				o.Salary == command.Salary &&
				o.Currency == command.Currency &&
				o.Status == "Draft" &&
				o.CreatedBy == command.CreatedBy)),
			Times.Once);
	}

	[Fact]
	public async Task Handle_WhenAddReturnsZero_ReturnsBadRequest()
	{
		var command = CreateValidCommand();

		_repositoryMock
			.Setup(r => r.AddAsync(It.IsAny<Offers>()))
			.ReturnsAsync(0);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsFailure);
		Assert.Equal((int)HttpResponseStatusCodes.BadRequest, result.Status);
		Assert.Equal(Error.EntityCouldNotBeCreated("Offers").Message, result.Error!.Message);
	}

	private static AddOffersCommand CreateValidCommand() => new()
	{
		ApplicationId = 1,
		CandidateId = 2,
		JobId = 3,
		OrganizationId = 4,
		Position = "Senior Developer",
		Salary = 120000m,
		Currency = "USD",
		JoiningDate = new DateTime(2026, 8, 1),
		Benefits = "Health insurance, 401k",
		ExpirationDate = new DateTime(2026, 7, 15),
		CreatedBy = "recruiter"
	};
}
