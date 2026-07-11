using Organization.Application.Abstractions.Data;
using Organization.Application.Common;
using Organization.Domain.OrganizationMember;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Organization.Application.Features.OrganizationMember.Commands.UpdateOrganizationMembers
{
	public class UpdateOrganizationMembersCommandHandler : IRequestHandler<UpdateOrganizationMembersCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<OrganizationMembers> _repository;

		public UpdateOrganizationMembersCommandHandler(IGenericRepository<OrganizationMembers> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(UpdateOrganizationMembersCommand request, CancellationToken cancellationToken)
		{
			var conflictProp = new
			{
				OrganizationId = request.OrganizationId,
				UserId = request.UserId,
			};

			var conflict = (await _repository.GetByPropertiesAsync(conflictProp))
				.FirstOrDefault(m => m.Id != request.Id);

			if (conflict is not null)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.Conflict,
					Error.Conflict("Organization member with this user already exists for the organization"));
			}

			var member = await _repository.GetByIdAsync(request.Id);
			if (member is not null)
			{
				member.Update(
					request.FullName,
					request.Email,
					request.Phone,
					request.Role,
					request.IsActive,
					request.UpdatedBy);

				var affectedRows = await _repository.UpdateAsync(member);

				if (affectedRows == 0)
				{
					return Result.Failure<CommandResponse>(
						HttpResponseStatusCodes.NotFound,
						Error.NotFound("OrganizationMembers"));
				}

				return new CommandResponse { IsSuccess = true };
			}

			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("OrganizationMember"));
		}
	}
}
