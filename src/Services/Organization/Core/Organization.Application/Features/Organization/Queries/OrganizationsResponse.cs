using Organization.Application.Common.CRUD.Queries;

namespace Organization.Application.Features.Organization.Queries
{
	public class OrganizationsResponse : IGenericResponse
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string? Industry { get; set; }
		public string? Website { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string? Address { get; set; }
		public string? LogoUrl { get; set; }
		public bool IsActive { get; set; }
	}
}
