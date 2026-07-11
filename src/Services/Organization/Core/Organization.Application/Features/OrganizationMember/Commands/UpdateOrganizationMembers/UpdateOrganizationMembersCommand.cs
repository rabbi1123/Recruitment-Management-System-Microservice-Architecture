using Organization.Application.Abstractions.CRUD;
using Organization.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Organization.Application.Features.OrganizationMember.Commands.UpdateOrganizationMembers
{
	public record UpdateOrganizationMembersCommand : IRequest<Result<CommandResponse>>, IUpdatedByCommand
	{
		public long Id { get; set; }
		public long OrganizationId { get; set; }
		public long UserId { get; set; }
		public string FullName { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string Role { get; set; }
		public bool IsActive { get; set; }
		public string? UpdatedBy { get; set; }
	}
}
