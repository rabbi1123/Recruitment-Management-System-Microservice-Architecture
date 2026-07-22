using Recruitment.Application.Common;
using Recruitment.Application.Common.CRUD.Commands;
using Recruitment.Application.Common.CRUD.Queries;
using Recruitment.Application.Features.Offer.Commands.AddOffers;
using Recruitment.Application.Features.Offer.Commands.UpdateOffers;
using Recruitment.Application.Features.Offer.Queries;
using Recruitment.Domain.Offer;
using Recruitment.WebAPI.Controllers.Offer;
using Common.Platform.Domain.Abstractions;
using MediatR;
using Moq;
using Xunit;

namespace Recruitment.WebAPI.Tests.Controllers.Offer;

public class OffersControllerTests
{
	private readonly Mock<IMediator> _mediatorMock;
	private readonly OffersController _controller;

	public OffersControllerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_controller = new OffersController(_mediatorMock.Object);
	}

	[Fact]
	public async Task GetAll_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetAllQuery<OffersResponse>();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetAllQuery<OffersResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<OffersResponse>());

		var result = await _controller.GetAll(request);

		ControllerTestAssertions.AssertOkResult<List<OffersResponse>>(result);
	}

	[Fact]
	public async Task GetById_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new GetByIdQuery<OffersResponse> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<GetByIdQuery<OffersResponse>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new OffersResponse { Id = 1 });

		var result = await _controller.GetById(request);

		ControllerTestAssertions.AssertOkResult<OffersResponse>(result);
	}

	[Fact]
	public async Task Create_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new AddOffersCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<AddOffersCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Create(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Update_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new UpdateOffersCommand();
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<UpdateOffersCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Update(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}

	[Fact]
	public async Task Delete_WhenMediatorSucceeds_ReturnsOk()
	{
		var request = new DeleteCommand<Offers> { Id = 1 };
		_mediatorMock
			.Setup(m => m.Send(It.IsAny<DeleteCommand<Offers>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new CommandResponse { IsSuccess = true });

		var result = await _controller.Delete(request);

		ControllerTestAssertions.AssertOkResult<CommandResponse>(result);
	}
}
