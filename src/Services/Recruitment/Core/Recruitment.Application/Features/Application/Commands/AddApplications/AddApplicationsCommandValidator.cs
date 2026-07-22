using Recruitment.Application.Abstractions.Validation;
using Recruitment.Domain.Application;
using FluentValidation;

namespace Recruitment.Application.Features.Application.Commands.AddApplications
{
	public class AddApplicationsCommandValidator : AbstractValidator<AddApplicationsCommand>
	{
		public AddApplicationsCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.JobId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.CandidateId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.OrganizationId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.RecruiterId)
				.GreaterThan(0)
					.When(x => x.RecruiterId.HasValue)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.CreatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
