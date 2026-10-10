using Messaging.Contracts.Events.Email;
using Messaging.Contracts.Events.Push;
using Messaging.Contracts.Events.Sms;
using Notification.Application.Common;
using Notification.Application.Features.Notifications.Commands;

namespace Notification.Infrastructure.Messaging;

internal readonly record struct NotificationEventData(
	string TemplateName,
	string RelatedId,
	string? Email,
	Dictionary<string, string> Tokens);

internal static class PublishedEventMapper
{
	public static NotificationEventData From(EmployeeRegisteredEmailEvent message) =>
		FromEmployeeRegistered(message.UserId, message.Email);

	public static NotificationEventData From(UserVerificationEmailEvent message, string publicWebBaseUrl)
	{
		var baseUrl = publicWebBaseUrl.TrimEnd('/');
		var verificationUrl = $"{baseUrl}/verify-email?token={Uri.EscapeDataString(message.VerificationToken)}";

		return new NotificationEventData(
			NotificationTemplateNames.EmailVerification,
			message.UserId.ToString(),
			message.Email,
			new Dictionary<string, string>
			{
				["UserId"] = message.UserId.ToString(),
				["Email"] = message.Email,
				["VerificationUrl"] = verificationUrl
			});
	}

	public static NotificationEventData From(EmployeeRegisteredSmsEvent message) =>
		FromEmployeeRegistered(message.UserId, message.Email);

	public static NotificationEventData From(EmployeeRegisteredPushEvent message) =>
		FromEmployeeRegistered(message.UserId, message.Email);

	public static NotificationEventData From(OrganizationCreatedEmailEvent message) =>
		FromOrganizationCreated(message.OrganizationId, message.Name, message.OccurredOnUtc);

	public static NotificationEventData From(OrganizationCreatedSmsEvent message) =>
		FromOrganizationCreated(message.OrganizationId, message.Name, message.OccurredOnUtc);

	public static NotificationEventData From(OrganizationCreatedPushEvent message) =>
		FromOrganizationCreated(message.OrganizationId, message.Name, message.OccurredOnUtc);

	public static NotificationEventData From(JobPublishedEmailEvent message) =>
		FromJobPublished(message.JobId, message.OrganizationId, message.Title, message.PublishedOnUtc);

	public static NotificationEventData From(JobPublishedSmsEvent message) =>
		FromJobPublished(message.JobId, message.OrganizationId, message.Title, message.PublishedOnUtc);

	public static NotificationEventData From(JobPublishedPushEvent message) =>
		FromJobPublished(message.JobId, message.OrganizationId, message.Title, message.PublishedOnUtc);

	public static NotificationEventData From(ApplicationSubmittedEmailEvent message) =>
		FromApplicationSubmitted(message.ApplicationId, message.JobId, message.CandidateId, message.OrganizationId, message.OccurredOnUtc);

	public static NotificationEventData From(ApplicationSubmittedSmsEvent message) =>
		FromApplicationSubmitted(message.ApplicationId, message.JobId, message.CandidateId, message.OrganizationId, message.OccurredOnUtc);

	public static NotificationEventData From(ApplicationSubmittedPushEvent message) =>
		FromApplicationSubmitted(message.ApplicationId, message.JobId, message.CandidateId, message.OrganizationId, message.OccurredOnUtc);

	public static NotificationEventData From(OfferSentEmailEvent message) =>
		FromOfferSent(message.OfferId, message.ApplicationId, message.CandidateId, message.JobId, message.OrganizationId, message.SentOnUtc);

	public static NotificationEventData From(OfferSentSmsEvent message) =>
		FromOfferSent(message.OfferId, message.ApplicationId, message.CandidateId, message.JobId, message.OrganizationId, message.SentOnUtc);

	public static NotificationEventData From(OfferSentPushEvent message) =>
		FromOfferSent(message.OfferId, message.ApplicationId, message.CandidateId, message.JobId, message.OrganizationId, message.SentOnUtc);

	public static SendEmailNotificationCommand ToEmail(Guid eventId, NotificationEventData data) => new()
	{
		EventId = eventId,
		TemplateName = data.TemplateName,
		Recipient = NotificationRecipient.Resolve(NotificationChannels.Email, data.TemplateName, data.RelatedId, data.Email),
		Tokens = data.Tokens
	};

	public static SendSmsNotificationCommand ToSms(Guid eventId, NotificationEventData data) => new()
	{
		EventId = eventId,
		TemplateName = data.TemplateName,
		Recipient = NotificationRecipient.Resolve(NotificationChannels.Sms, data.TemplateName, data.RelatedId, data.Email),
		Tokens = data.Tokens
	};

	public static SendPushNotificationCommand ToPush(Guid eventId, NotificationEventData data) => new()
	{
		EventId = eventId,
		TemplateName = data.TemplateName,
		Recipient = NotificationRecipient.Resolve(NotificationChannels.Push, data.TemplateName, data.RelatedId, data.Email),
		Tokens = data.Tokens
	};

	private static NotificationEventData FromEmployeeRegistered(Guid userId, string email) => new(
		NotificationTemplateNames.EmployeeRegistered,
		userId.ToString(),
		email,
		new Dictionary<string, string>
		{
			["UserId"] = userId.ToString(),
			["Email"] = email
		});

	private static NotificationEventData FromOrganizationCreated(long organizationId, string name, DateTime occurredOnUtc) => new(
		NotificationTemplateNames.OrganizationCreated,
		organizationId.ToString(),
		null,
		new Dictionary<string, string>
		{
			["OrganizationId"] = organizationId.ToString(),
			["Name"] = name,
			["OccurredOnUtc"] = occurredOnUtc.ToString("O")
		});

	private static NotificationEventData FromJobPublished(long jobId, long organizationId, string title, DateTime publishedOnUtc) => new(
		NotificationTemplateNames.JobPublished,
		jobId.ToString(),
		null,
		new Dictionary<string, string>
		{
			["JobId"] = jobId.ToString(),
			["OrganizationId"] = organizationId.ToString(),
			["Title"] = title,
			["PublishedOnUtc"] = publishedOnUtc.ToString("O")
		});

	private static NotificationEventData FromApplicationSubmitted(
		long applicationId,
		long jobId,
		long candidateId,
		long organizationId,
		DateTime occurredOnUtc) => new(
		NotificationTemplateNames.ApplicationSubmitted,
		applicationId.ToString(),
		null,
		new Dictionary<string, string>
		{
			["ApplicationId"] = applicationId.ToString(),
			["JobId"] = jobId.ToString(),
			["CandidateId"] = candidateId.ToString(),
			["OrganizationId"] = organizationId.ToString(),
			["OccurredOnUtc"] = occurredOnUtc.ToString("O")
		});

	private static NotificationEventData FromOfferSent(
		long offerId,
		long applicationId,
		long candidateId,
		long jobId,
		long organizationId,
		DateTime sentOnUtc) => new(
		NotificationTemplateNames.OfferSent,
		offerId.ToString(),
		null,
		new Dictionary<string, string>
		{
			["OfferId"] = offerId.ToString(),
			["ApplicationId"] = applicationId.ToString(),
			["CandidateId"] = candidateId.ToString(),
			["JobId"] = jobId.ToString(),
			["OrganizationId"] = organizationId.ToString(),
			["SentOnUtc"] = sentOnUtc.ToString("O")
		});
}
