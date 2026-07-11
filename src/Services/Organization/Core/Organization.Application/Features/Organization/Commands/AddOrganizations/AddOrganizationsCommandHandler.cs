using Organization.Application.Abstractions.Data;
using Organization.Application.Common;
using Organization.Domain.Organization;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Organization.Application.Features.Organization.Commands.AddOrganizations
{
	public class AddOrganizationsCommandHandler : IRequestHandler<AddOrganizationsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Organizations> _repository;

		public AddOrganizationsCommandHandler(IGenericRepository<Organizations> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(AddOrganizationsCommand request, CancellationToken cancellationToken)
		{
			var organization = Organizations.Create(
				request.Name,
				request.Industry,
				request.Website,
				request.Email,
				request.Phone,
				request.Address,
				request.LogoUrl,
				request.CreatedBy);

			var affectedRows = await _repository.AddAsync(organization);

			if (affectedRows == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("Organizations"));
			}

			return new CommandResponse { IsSuccess = true };
		}
	}
}
