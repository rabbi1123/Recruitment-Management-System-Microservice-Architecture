using Job.Application.Abstractions.CRUD;
using Job.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Job.Application.Features.JobSkill.Commands.UpdateJobSkills
{
	public record UpdateJobSkillsCommand : IRequest<Result<CommandResponse>>, IUpdatedByCommand
	{
		public long Id { get; set; }
		public long JobId { get; set; }
		public string SkillName { get; set; }
		public bool IsRequired { get; set; }
		public bool IsActive { get; set; }
		public string? UpdatedBy { get; set; }
	}
}
