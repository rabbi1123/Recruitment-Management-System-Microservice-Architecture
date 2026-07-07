using Candidate.Application.Abstractions.CRUD;
using Candidate.Application.Common;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Candidate.Application.Features.CandidateEducation.Commands.UpdateCandidateEducations
{
	public record UpdateCandidateEducationsCommand : IRequest<Result<CommandResponse>>, IUpdatedByCommand
	{
		public long Id { get; set; }
		public long CandidateId { get; set; }
		public string Institution { get; set; }
		public string? Degree { get; set; }
		public string? FieldOfStudy { get; set; }
		public DateOnly? StartDate { get; set; }
		public DateOnly? EndDate { get; set; }
		public string? Grade { get; set; }
		public bool IsActive { get; set; }
		public string? UpdatedBy { get; set; }
	}
}
