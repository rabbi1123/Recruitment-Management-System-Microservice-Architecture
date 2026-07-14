using Job.Application.Abstractions.Validation;
using FluentValidation;

namespace Job.Application.Features.SavedJob.Commands.AddSavedJobs
{
	public class AddSavedJobsCommandValidator : AbstractValidator<AddSavedJobsCommand>
	{
		public AddSavedJobsCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.CandidateId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.JobId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.CreatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
