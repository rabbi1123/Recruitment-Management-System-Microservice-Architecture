using Candidate.Application.Abstractions.Validation;
using FluentValidation;

namespace Candidate.Application.Features.CandidateCertification.Commands.UpdateCandidateCertifications
{
	public class UpdateCandidateCertificationsCommandValidator : AbstractValidator<UpdateCandidateCertificationsCommand>
	{
		public UpdateCandidateCertificationsCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.Id)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.CandidateId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.Name)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(200)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.IssuingOrg)
				.MaximumLength(200)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.When(x => !string.IsNullOrWhiteSpace(x.IssuingOrg))
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.ExpiryDate)
				.GreaterThan(x => x.IssueDate!.Value)
					.When(x => x.IssueDate.HasValue && x.ExpiryDate.HasValue)
					.WithMessage(catalog.Messages["EndDateAfterStart"]);

			RuleFor(x => x.CredentialId)
				.MaximumLength(150)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["AlphaNumericHyphenSlash"])
					.When(x => !string.IsNullOrWhiteSpace(x.CredentialId))
					.WithMessage(catalog.Messages["AlphaNumericHyphenSlash"]);

			RuleFor(x => x.CredentialUrl)
				.MaximumLength(256)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["Url"])
					.When(x => !string.IsNullOrWhiteSpace(x.CredentialUrl))
					.WithMessage(catalog.Messages["UrlInvalid"]);

			RuleFor(x => x.UpdatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
