using Organization.Application.Abstractions.Mapping;
using Organization.Application.Features.Organization.Queries;
using Organization.Domain.Organization;

namespace Organization.Application.Features.Organization
{
	public class OrganizationsMapper : IResponseEntityMapper<Organizations, OrganizationsResponse>
	{
		public OrganizationsResponse MapToResponse(Organizations entity)
		{
			return new OrganizationsResponse
			{
				Id = entity.Id,
				Name = entity.Name,
				Industry = entity.Industry,
				Website = entity.Website,
				Email = entity.Email,
				Phone = entity.Phone,
				Address = entity.Address,
				LogoUrl = entity.LogoUrl,
				IsActive = entity.IsActive
			};
		}
	}
}
