using Organization.Application.Abstractions.CRUD;
using Organization.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Organization.Application.Features.Organization.Commands.UpdateOrganizations
{
	public record UpdateOrganizationsCommand : IRequest<Result<CommandResponse>>, IUpdatedByCommand
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
		public string? UpdatedBy { get; set; }
	}
}
