using Notification.Application.Common;
using Notification.Application.Interfaces;

namespace Notification.Infrastructure.Email;

public sealed class EmailTemplateService : IEmailTemplateService
{
	public NotificationContent Render(string templateName, IReadOnlyDictionary<string, string> tokens)
	{
		return templateName switch
		{
			NotificationTemplateNames.EmployeeRegistered => Content(
				"Employee registered",
				$"Employee {Token(tokens, "Email")} registered."),
			NotificationTemplateNames.EmailVerification => Content(
				"Verify your email address",
				$"Welcome.\nPlease verify your email address.\n{Token(tokens, "VerificationUrl")}"),
			NotificationTemplateNames.OrganizationCreated => Content(
				"Organization created",
				$"Organization {Token(tokens, "Name")} ({Token(tokens, "OrganizationId")}) was created."),
			NotificationTemplateNames.JobPublished => Content(
				"Job published",
				$"Job {Token(tokens, "Title")} ({Token(tokens, "JobId")}) was published for organization {Token(tokens, "OrganizationId")}."),
			NotificationTemplateNames.ApplicationSubmitted => Content(
				"Application submitted",
				$"Application {Token(tokens, "ApplicationId")} submitted for job {Token(tokens, "JobId")} by candidate {Token(tokens, "CandidateId")}."),
			NotificationTemplateNames.OfferSent => Content(
				"Offer sent",
				$"Offer {Token(tokens, "OfferId")} sent for application {Token(tokens, "ApplicationId")}."),
			_ => throw new InvalidOperationException($"Unknown notification template '{templateName}'.")
		};
	}

	private static NotificationContent Content(string heading, string body) => new(heading, heading, body);

	private static string Token(IReadOnlyDictionary<string, string> tokens, string name) =>
		tokens.TryGetValue(name, out var value) ? value : string.Empty;
}
