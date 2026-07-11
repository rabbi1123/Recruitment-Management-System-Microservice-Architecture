using Organization.Application.Abstractions.Validation;
using FluentValidation;

namespace Organization.Application.Features.OrganizationMember.Commands.AddOrganizationMembers
{
	public class AddOrganizationMembersCommandValidator : AbstractValidator<AddOrganizationMembersCommand>
	{
		private static readonly string[] Roles = ["OrganizationAdmin", "Recruiter"];

		public AddOrganizationMembersCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.OrganizationId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.UserId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.FullName)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(200)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.WithMessage(catalog.Messages["EnglishNameRule"]);

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

			RuleFor(x => x.Role)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.Must(x => Roles.Contains(x))
					.WithMessage(catalog.OneOf("OrganizationAdmin", "Recruiter"));

			RuleFor(x => x.CreatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
