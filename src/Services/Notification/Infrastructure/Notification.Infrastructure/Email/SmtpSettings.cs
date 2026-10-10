using System.ComponentModel.DataAnnotations;

namespace Notification.Infrastructure.Email;

public sealed class SmtpSettings
{
	public const string SectionName = "Smtp";

	[Required]
	public string Host { get; set; } = "localhost";

	[Range(1, 65535)]
	public int Port { get; set; } = 1025;

	public string Username { get; set; } = string.Empty;

	public string Password { get; set; } = string.Empty;

	[Required]
	public string From { get; set; } = "noreply@recruitment.local";

	public bool EnableSsl { get; set; }
}
