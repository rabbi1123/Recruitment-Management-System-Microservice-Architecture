using Job.Application.Abstractions.Auth;
using Job.Application.Common.Auth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Job.Infrastructure.Auth
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private const string SystemUsername = "System";

        public bool IsAuthenticated { get; }
        public long UserId { get; }
        public string Username { get; }
        public string FullName { get; }
        public string Email { get; }
        public long? EmployeeId { get; }
        public bool TwoFactorRequired { get; }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                IsAuthenticated = true;

                FullName = user.FindFirstValue(ClaimTypes.Name);
                Email = user.FindFirstValue(ClaimTypes.Email);

                var usernameClaim = user.FindFirstValue(CustomClaimTypes.Username);

                Username = string.IsNullOrWhiteSpace(usernameClaim)
                    ? SystemUsername
                    : usernameClaim;

                if (long.TryParse(
                    user.FindFirstValue(CustomClaimTypes.UserId),
                    out var userId))
                {
                    UserId = userId;
                }

                if (bool.TryParse(
                    user.FindFirstValue(CustomClaimTypes.TwoFactorRequired),
                    out var twoFactor))
                {
                    TwoFactorRequired = twoFactor;
                }
            }
            else
            {
                // ✅ Anonymous HTTP user
                Username = SystemUsername;
                IsAuthenticated = false;
            }
        }
    }
}
