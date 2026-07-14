using Job.Application.Abstractions.Validation;
using FluentValidation;

namespace Job.Application.Features.Job.Commands.UpdateJobs
{
	public class UpdateJobsCommandValidator : AbstractValidator<UpdateJobsCommand>
	{
		private static readonly string[] EmploymentTypes =
			["FullTime", "PartTime", "Contract", "Internship", "Temporary"];

		private static readonly string[] Statuses = ["Draft", "Published", "Closed"];

		public UpdateJobsCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.Id)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.OrganizationId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.RecruiterId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.Title)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(200)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.Department)
				.MaximumLength(100)
					.When(x => !string.IsNullOrWhiteSpace(x.Department))
					.WithMessage(catalog.Messages["MaxLength"]);

			RuleFor(x => x.EmploymentType)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.Must(x => EmploymentTypes.Contains(x))
					.WithMessage(catalog.OneOf("FullTime", "PartTime", "Contract", "Internship", "Temporary"));

			RuleFor(x => x.Location)
				.MaximumLength(200)
					.When(x => !string.IsNullOrWhiteSpace(x.Location))
					.WithMessage(catalog.Messages["MaxLength"]);

			RuleFor(x => x.ExperienceMin)
				.GreaterThanOrEqualTo(0)
					.When(x => x.ExperienceMin.HasValue)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.ExperienceMax)
				.GreaterThanOrEqualTo(0)
					.When(x => x.ExperienceMax.HasValue)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x)
				.Must(x => !x.ExperienceMin.HasValue || !x.ExperienceMax.HasValue || x.ExperienceMax >= x.ExperienceMin)
					.WithMessage("ExperienceMax must be greater than or equal to ExperienceMin.");

			RuleFor(x => x)
				.Must(x => !x.SalaryMin.HasValue || !x.SalaryMax.HasValue || x.SalaryMax >= x.SalaryMin)
					.WithMessage("SalaryMax must be greater than or equal to SalaryMin.");

			RuleFor(x => x.Currency)
				.MaximumLength(3)
					.When(x => !string.IsNullOrWhiteSpace(x.Currency))
					.WithMessage(catalog.Messages["MaxLength"]);

			RuleFor(x => x.Status)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.Must(x => Statuses.Contains(x))
					.WithMessage(catalog.OneOf("Draft", "Published", "Closed"));

			RuleFor(x => x.UpdatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
