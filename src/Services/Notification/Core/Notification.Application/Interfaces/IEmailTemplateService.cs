namespace Notification.Application.Interfaces;

public interface IEmailTemplateService
{
	NotificationContent Render(string templateName, IReadOnlyDictionary<string, string> tokens);
}
