using Organization.Application.Abstractions.CRUD;
using Organization.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Organization.Application.Features.OrganizationMember.Commands.AddOrganizationMembers
{
	public record AddOrganizationMembersCommand : IRequest<Result<CommandResponse>>, ICreatedByCommand
	{
		public long OrganizationId { get; set; }
		public long UserId { get; set; }
		public string FullName { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string Role { get; set; }
		public string? CreatedBy { get; set; }
	}
}
