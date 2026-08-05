namespace Identity.Domain.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Recruiter = "Recruiter";
    public const string Employee = "Employee";

    public static readonly IReadOnlyCollection<string> All =
    [
        Admin,
        Recruiter,
        Employee
    ];
}
