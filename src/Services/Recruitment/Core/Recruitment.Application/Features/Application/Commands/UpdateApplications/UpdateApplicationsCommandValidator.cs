using Recruitment.Application.Abstractions.Validation;
using Recruitment.Domain.Application;
using FluentValidation;

namespace Recruitment.Application.Features.Application.Commands.UpdateApplications
{
	public class UpdateApplicationsCommandValidator : AbstractValidator<UpdateApplicationsCommand>
	{
		public UpdateApplicationsCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.Id)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.RecruiterId)
				.GreaterThan(0)
					.When(x => x.RecruiterId.HasValue)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.CurrentStage)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(30)
					.WithMessage(catalog.Messages["MaxLength"])
				.Must(stage => Applications.AllowedStages.Contains(stage))
					.WithMessage("CurrentStage must be a valid stage value");

			RuleFor(x => x.Status)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(20)
					.WithMessage(catalog.Messages["MaxLength"])
				.Must(status => Applications.AllowedStatuses.Contains(status))
					.WithMessage("Status must be a valid status value");

			RuleFor(x => x.RejectionReason)
				.MaximumLength(512)
					.When(x => !string.IsNullOrWhiteSpace(x.RejectionReason))
					.WithMessage(catalog.Messages["MaxLength"]);

			RuleFor(x => x.UpdatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
