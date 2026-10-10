namespace Notification.Application.Common;

public static class NotificationRecipient
{
	public static string Resolve(string channel, string templateName, string relatedId, string? email = null)
	{
		if (channel == NotificationChannels.Email
			&& (templateName == NotificationTemplateNames.EmployeeRegistered
				|| templateName == NotificationTemplateNames.EmailVerification)
			&& !string.IsNullOrWhiteSpace(email))
		{
			return email;
		}

		return $"{templateName}:{relatedId}";
	}
}
