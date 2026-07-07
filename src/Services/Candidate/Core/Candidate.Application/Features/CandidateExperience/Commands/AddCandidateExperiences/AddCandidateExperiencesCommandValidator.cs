using Candidate.Application.Abstractions.Validation;
using FluentValidation;

namespace Candidate.Application.Features.CandidateExperience.Commands.AddCandidateExperiences
{
	public class AddCandidateExperiencesCommandValidator : AbstractValidator<AddCandidateExperiencesCommand>
	{
		public AddCandidateExperiencesCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.CandidateId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.Company)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(200)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.Title)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(150)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.Location)
				.MaximumLength(150)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.EndDate)
				.GreaterThan(x => x.StartDate)
					.When(x => x.EndDate.HasValue)
					.WithMessage(catalog.Messages["EndDateAfterStart"]);

			RuleFor(x => x.EndDate)
				.Null()
					.When(x => x.IsCurrent)
					.WithMessage("End date must be empty when the experience is current.");

			RuleFor(x => x.Description)
				.Matches(catalog.Patterns["EnglishName"])
					.When(x => !string.IsNullOrWhiteSpace(x.Description))
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.CreatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
