using Organization.Application.Abstractions.Mapping;
using Organization.Application.Features.OrganizationMember.Queries;
using Organization.Domain.OrganizationMember;

namespace Organization.Application.Features.OrganizationMember
{
	public class OrganizationMembersMapper : IResponseEntityMapper<OrganizationMembers, OrganizationMembersResponse>
	{
		public OrganizationMembersResponse MapToResponse(OrganizationMembers entity)
		{
			return new OrganizationMembersResponse
			{
				Id = entity.Id,
				OrganizationId = entity.OrganizationId,
				UserId = entity.UserId,
				FullName = entity.FullName,
				Email = entity.Email,
				Phone = entity.Phone,
				Role = entity.Role,
				IsActive = entity.IsActive
			};
		}
	}
}
