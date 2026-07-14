using Job.Application.Abstractions.CRUD;
using Job.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Job.Application.Features.JobSkill.Commands.AddJobSkills
{
	public record AddJobSkillsCommand : IRequest<Result<CommandResponse>>, ICreatedByCommand
	{
		public long JobId { get; set; }
		public string SkillName { get; set; }
		public bool IsRequired { get; set; } = true;
		public string? CreatedBy { get; set; }
	}
}
