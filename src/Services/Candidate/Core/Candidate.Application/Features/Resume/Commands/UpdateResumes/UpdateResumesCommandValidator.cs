using Candidate.Application.Abstractions.Validation;
using FluentValidation;

namespace Candidate.Application.Features.Resume.Commands.UpdateResumes
{
	public class UpdateResumesCommandValidator : AbstractValidator<UpdateResumesCommand>
	{
		private static readonly string[] FileFormats = ["PDF", "DOCX"];
		private const long MaxFileSize = 10_485_760;

		public UpdateResumesCommandValidator(IValidationCatalog catalog)
		{
			RuleFor(x => x.Id)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.CandidateId)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"]);

			RuleFor(x => x.FileName)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(256)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["EnglishName"])
					.WithMessage(catalog.Messages["EnglishNameRule"]);

			RuleFor(x => x.FileUrl)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.MaximumLength(512)
					.WithMessage(catalog.Messages["MaxLength"])
				.Matches(catalog.Patterns["Url"])
					.WithMessage(catalog.Messages["UrlInvalid"]);

			RuleFor(x => x.FileFormat)
				.NotEmpty()
					.WithMessage(catalog.Messages["Required"])
				.Must(x => FileFormats.Contains(x))
					.WithMessage(catalog.OneOf("PDF", "DOCX"));

			RuleFor(x => x.FileSize)
				.GreaterThan(0)
					.WithMessage(catalog.Messages["PositiveNumber"])
				.LessThanOrEqualTo(MaxFileSize)
					.WithMessage(catalog.Messages["MaxValue"]);

			RuleFor(x => x.UpdatedBy)
				.MaximumLength(100)
					.WithMessage(catalog.Messages["MaxLength"]);
		}
	}
}
