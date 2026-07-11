namespace Organization.Application.Abstractions.Auth
{
    public interface ICurrentUserService
    {
        bool IsAuthenticated { get; }

        long UserId { get; }
        string Username { get; }
        string FullName { get; }
        string Email { get; }
        long? EmployeeId { get; }

        bool TwoFactorRequired { get; }
    }
}
