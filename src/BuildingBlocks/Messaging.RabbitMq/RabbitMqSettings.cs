using System.ComponentModel.DataAnnotations;

namespace Messaging.RabbitMq;

public sealed class RabbitMqSettings
{
    public const string SectionName = "RabbitMq";

    [Required]
    public string Host { get; set; } = "localhost";

    [Required]
    public string Username { get; set; } = "guest";

    [Required]
    public string Password { get; set; } = "guest";
}
