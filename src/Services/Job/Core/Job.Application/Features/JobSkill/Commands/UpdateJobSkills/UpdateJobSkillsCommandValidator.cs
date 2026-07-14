using Job.Application.Abstractions.Validation;
using FluentValidation;

namespace Job.Application.Features.JobSkill.Commands.UpdateJobSkills
{
	public class UpdateJobSkillsCommandValidator : AbstractValidator<UpdateJobSkillsCommand>
	{
		public UpdateJobSkillsCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.Id)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.JobId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.SkillName)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.UpdatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
