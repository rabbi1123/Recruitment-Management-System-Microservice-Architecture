using Job.Application.Abstractions.Validation;
using FluentValidation;

namespace Job.Application.Features.SavedJob.Commands.UpdateSavedJobs
{
	public class UpdateSavedJobsCommandValidator : AbstractValidator<UpdateSavedJobsCommand>
	{
		public UpdateSavedJobsCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.Id)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.CandidateId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.JobId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.UpdatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
