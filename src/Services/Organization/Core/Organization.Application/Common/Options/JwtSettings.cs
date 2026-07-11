using System.ComponentModel.DataAnnotations;

namespace Organization.Application.Common.Options
{
    public class JwtSettings
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Issuer { get; set; }

        [Required]
        public string Audience { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public int UserBanSeconds { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public int OtpExpiresInSeconds { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public int AccessTokenExpiryMinutes { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public int RefreshTokenExpiryDays { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public int MaxSendOtpRetryCount { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public int MaxReTryCountToVerify { get; set; }
    }
}
