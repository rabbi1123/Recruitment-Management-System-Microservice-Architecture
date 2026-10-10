using System.ComponentModel.DataAnnotations;

namespace Notification.Infrastructure.Email;

public sealed class PublicWebSettings
{
	public const string SectionName = "Notifications";

	[Required]
	public string PublicWebBaseUrl { get; set; } = "https://recruitment.com";
}
