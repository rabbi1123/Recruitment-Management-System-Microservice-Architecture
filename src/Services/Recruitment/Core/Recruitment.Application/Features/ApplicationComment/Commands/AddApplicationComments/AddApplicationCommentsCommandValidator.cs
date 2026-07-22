using Recruitment.Application.Abstractions.Validation;
using FluentValidation;

namespace Recruitment.Application.Features.ApplicationComment.Commands.AddApplicationComments
{
	public class AddApplicationCommentsCommandValidator : AbstractValidator<AddApplicationCommentsCommand>
	{
		public AddApplicationCommentsCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.ApplicationId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.AuthorId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.Comment)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"]);

			RuleFor(x => x.CreatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
