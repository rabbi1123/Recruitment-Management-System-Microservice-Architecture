using Candidate.Application.Abstractions.Validation;
using FluentValidation;

namespace Candidate.Application.Features.CandidateEducation.Commands.UpdateCandidateEducations
{
	public class UpdateCandidateEducationsCommandValidator : AbstractValidator<UpdateCandidateEducationsCommand>
	{
		public UpdateCandidateEducationsCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.Id)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.CandidateId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.Institution)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(200)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.Degree)
				.MaximumLength(150)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.When(x => !string.IsNullOrWhiteSpace(x.Degree))
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.FieldOfStudy)
				.MaximumLength(150)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.When(x => !string.IsNullOrWhiteSpace(x.FieldOfStudy))
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.EndDate)
				.GreaterThan(x => x.StartDate!.Value)
					.When(x => x.StartDate.HasValue && x.EndDate.HasValue)
					.WithMessage(catalog.Messages["EndDateAfterStart"]);

			RuleFor(x => x.Grade)
				.MaximumLength(50)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.When(x => !string.IsNullOrWhiteSpace(x.Grade))
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.UpdatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
