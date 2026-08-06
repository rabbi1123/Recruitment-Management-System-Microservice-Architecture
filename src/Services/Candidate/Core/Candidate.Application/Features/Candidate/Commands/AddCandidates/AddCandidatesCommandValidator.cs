using Candidate.Application.Abstractions.Validation;
using FluentValidation;

namespace Candidate.Application.Features.Candidate.Commands.AddCandidates
{
	public class AddCandidatesCommandValidator : AbstractValidator<AddCandidatesCommand>
	{
		public AddCandidatesCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.UserId)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"]);

			RuleFor(x => x.Email)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(256)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EmailList"])
					.WithMessage(catalog.Messages["EmailListRule"]);

			RuleFor(x => x.Phone)
				.MaximumLength(20)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["PhoneList"])
					.WithMessage(catalog.Messages["PhoneListRule"]);

			RuleFor(x => x.Address)
				.MaximumLength(512)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.LinkedinProfile)
				.MaximumLength(256)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["Url"])
					.WithMessage(catalog.Messages["UrlInvalid"]);

			RuleFor(x => x.PortfolioUrl)
				.MaximumLength(256)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["Url"])
					.WithMessage(catalog.Messages["UrlInvalid"]);

			RuleFor(x => x.Headline)
				.MaximumLength(200)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.Summary)
				.Matches(catalog.Patterns["EnglishName"])
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.CreatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
