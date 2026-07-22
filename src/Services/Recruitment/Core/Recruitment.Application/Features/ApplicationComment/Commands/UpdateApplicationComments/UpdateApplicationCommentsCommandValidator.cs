using Recruitment.Application.Abstractions.Validation;
using FluentValidation;

namespace Recruitment.Application.Features.ApplicationComment.Commands.UpdateApplicationComments
{
	public class UpdateApplicationCommentsCommandValidator : AbstractValidator<UpdateApplicationCommentsCommand>
	{
		public UpdateApplicationCommentsCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.Id)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.Comment)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"]);

			RuleFor(x => x.UpdatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
