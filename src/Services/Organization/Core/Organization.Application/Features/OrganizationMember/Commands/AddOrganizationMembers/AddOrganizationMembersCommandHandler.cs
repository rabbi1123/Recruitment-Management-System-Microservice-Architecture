using Organization.Application.Abstractions.Data;
using Organization.Application.Common;
using Organization.Domain.OrganizationMember;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Organization.Application.Features.OrganizationMember.Commands.AddOrganizationMembers
{
	public class AddOrganizationMembersCommandHandler : IRequestHandler<AddOrganizationMembersCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<OrganizationMembers> _repository;

		public AddOrganizationMembersCommandHandler(IGenericRepository<OrganizationMembers> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(AddOrganizationMembersCommand request, CancellationToken cancellationToken)
		{
			var conflictProp = new
			{
				OrganizationId = request.OrganizationId,
				UserId = request.UserId,
			};

			var conflict = (await _repository.GetByPropertiesAsync(conflictProp)).FirstOrDefault();

			if (conflict is not null)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.Conflict,
					Error.Conflict("Organization member with this user already exists for the organization"));
			}

			var member = OrganizationMembers.Create(
				request.OrganizationId,
				request.UserId,
				request.FullName,
				request.Email,
				request.Phone,
				request.Role,
				request.CreatedBy);

			var affectedRows = await _repository.AddAsync(member);

			if (affectedRows == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("OrganizationMembers"));
			}

			return new CommandResponse { IsSuccess = true };
		}
	}
}
