using Organization.Application.Abstractions.Validation;
using FluentValidation;

namespace Organization.Application.Features.Organization.Commands.AddOrganizations
{
	public class AddOrganizationsCommandValidator : AbstractValidator<AddOrganizationsCommand>
	{
		public AddOrganizationsCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.Name)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(200)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.Industry)
				.MaximumLength(100)
					.When(x => !string.IsNullOrWhiteSpace(x.Industry))
					.WithMessage(catalog.Messages["MaxLength"]);

			RuleFor(x => x.Website)
				.MaximumLength(256)
					.When(x => !string.IsNullOrWhiteSpace(x.Website))
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["Url"])
					.When(x => !string.IsNullOrWhiteSpace(x.Website))
					.WithMessage(catalog.Messages["UrlInvalid"]);

			RuleFor(x => x.Email)
				.MaximumLength(256)
					.When(x => !string.IsNullOrWhiteSpace(x.Email))
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EmailList"])
					.When(x => !string.IsNullOrWhiteSpace(x.Email))
					.WithMessage(catalog.Messages["EmailListRule"]);

			RuleFor(x => x.Phone)
				.MaximumLength(20)
					.When(x => !string.IsNullOrWhiteSpace(x.Phone))
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["PhoneList"])
					.When(x => !string.IsNullOrWhiteSpace(x.Phone))
					.WithMessage(catalog.Messages["PhoneListRule"]);

			RuleFor(x => x.Address)
				.MaximumLength(512)
					.When(x => !string.IsNullOrWhiteSpace(x.Address))
					.WithMessage(catalog.Messages["MaxLength"]);

			RuleFor(x => x.LogoUrl)
				.MaximumLength(512)
					.When(x => !string.IsNullOrWhiteSpace(x.LogoUrl))
					.WithMessage(catalog.Messages["MaxLength"]);

			RuleFor(x => x.CreatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
