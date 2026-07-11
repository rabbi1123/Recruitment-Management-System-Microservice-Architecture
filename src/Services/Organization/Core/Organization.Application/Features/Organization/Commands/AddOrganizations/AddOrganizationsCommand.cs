using Organization.Application.Abstractions.CRUD;
using Organization.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Organization.Application.Features.Organization.Commands.AddOrganizations
{
	public record AddOrganizationsCommand : IRequest<Result<CommandResponse>>, ICreatedByCommand
	{
		public string Name { get; set; }
		public string? Industry { get; set; }
		public string? Website { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string? Address { get; set; }
		public string? LogoUrl { get; set; }
		public string? CreatedBy { get; set; }
	}
}
