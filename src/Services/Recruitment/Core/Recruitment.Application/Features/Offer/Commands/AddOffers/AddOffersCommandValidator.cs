using Recruitment.Application.Abstractions.Validation;
using FluentValidation;

namespace Recruitment.Application.Features.Offer.Commands.AddOffers
{
	public class AddOffersCommandValidator : AbstractValidator<AddOffersCommand>
	{
		public AddOffersCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.ApplicationId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.CandidateId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.JobId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.OrganizationId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.Position)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(200)
					.WithMessage(catalog.Messages["MaxLength"]);

			RuleFor(x => x.Salary)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.Currency)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.Length(3)
					.WithMessage(catalog.Messages["MaxLength"]);

			RuleFor(x => x.CreatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
