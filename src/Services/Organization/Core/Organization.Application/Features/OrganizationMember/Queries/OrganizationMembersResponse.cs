using Organization.Application.Common.CRUD.Queries;

namespace Organization.Application.Features.OrganizationMember.Queries
{
	public class OrganizationMembersResponse : IGenericResponse
	{
		public long Id { get; set; }
		public long OrganizationId { get; set; }
		public long UserId { get; set; }
		public string FullName { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string Role { get; set; }
		public bool IsActive { get; set; }
	}
}
