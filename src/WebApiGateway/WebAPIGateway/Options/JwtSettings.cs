using System.ComponentModel.DataAnnotations;

namespace WebAPIGateway.Options;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Required]
    [MinLength(32)]
    public string SecretKey { get; set; } = string.Empty;
}
