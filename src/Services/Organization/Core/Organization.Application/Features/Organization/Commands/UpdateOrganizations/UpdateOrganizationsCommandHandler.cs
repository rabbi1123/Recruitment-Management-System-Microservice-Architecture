using Organization.Application.Abstractions.Data;
using Organization.Application.Common;
using Organization.Domain.Organization;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Organization.Application.Features.Organization.Commands.UpdateOrganizations
{
	public class UpdateOrganizationsCommandHandler : IRequestHandler<UpdateOrganizationsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<Organizations> _repository;

		public UpdateOrganizationsCommandHandler(IGenericRepository<Organizations> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(UpdateOrganizationsCommand request, CancellationToken cancellationToken)
		{
			var organization = await _repository.GetByIdAsync(request.Id);
			if (organization is not null)
			{
				organization.Update(
					request.Name,
					request.Industry,
					request.Website,
					request.Email,
					request.Phone,
					request.Address,
					request.LogoUrl,
					request.IsActive,
					request.UpdatedBy);

				var affectedRows = await _repository.UpdateAsync(organization);

				if (affectedRows == 0)
				{
					return Result.Failure<CommandResponse>(
						HttpResponseStatusCodes.NotFound,
						Error.NotFound("Organizations"));
				}

				return new CommandResponse { IsSuccess = true };
			}

			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("Organization"));
		}
	}
}
