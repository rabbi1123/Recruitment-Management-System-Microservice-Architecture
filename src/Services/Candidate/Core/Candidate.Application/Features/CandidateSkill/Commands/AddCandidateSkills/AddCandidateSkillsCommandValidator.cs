using Candidate.Application.Abstractions.Validation;
using FluentValidation;

namespace Candidate.Application.Features.CandidateSkill.Commands.AddCandidateSkills
{
	public class AddCandidateSkillsCommandValidator : AbstractValidator<AddCandidateSkillsCommand>
	{
		private static readonly string[] ProficiencyLevels = ["Beginner", "Intermediate", "Advanced", "Expert"];

		public AddCandidateSkillsCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.CandidateId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.Name)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.ProficiencyLevel)
				.Must(x => x is null || ProficiencyLevels.Contains(x))
					.WithMessage(catalog.OneOf("Beginner", "Intermediate", "Advanced", "Expert"));

			RuleFor(x => x.YearsOfExperience)
				.GreaterThanOrEqualTo(0)
					.When(x => x.YearsOfExperience.HasValue)
					.WithMessage(catalog.Messages["GteZero"])
				.LessThanOrEqualTo(999.9m)
					.When(x => x.YearsOfExperience.HasValue)
					.WithMessage(catalog.Messages["MaxValue"]);

			RuleFor(x => x.CreatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
