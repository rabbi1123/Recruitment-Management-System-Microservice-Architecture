using Recruitment.Application.Abstractions.Validation;
using Recruitment.Domain.Offer;
using FluentValidation;

namespace Recruitment.Application.Features.Offer.Commands.UpdateOffers
{
	public class UpdateOffersCommandValidator : AbstractValidator<UpdateOffersCommand>
	{
		public UpdateOffersCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.Id)
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

			RuleFor(x => x.Status)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(20)
					.WithMessage(catalog.Messages["MaxLength"])
				.Must(status => Offers.AllowedStatuses.Contains(status))
					.WithMessage("Status must be a valid offer status value");

			RuleFor(x => x.UpdatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
