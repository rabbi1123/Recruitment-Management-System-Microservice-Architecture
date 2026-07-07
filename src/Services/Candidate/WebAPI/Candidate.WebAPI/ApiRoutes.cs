namespace Candidate.WebAPI
{
    public static class ApiRoutes
    {
        public static class Common
        {
            public const string GetById = "get-by-id";
            public const string GetAll = "get-list";
            public const string Create = "create";
            public const string Update = "update";
            public const string Delete = "delete";
            public const string Upsert = "upsert";
            public const string Verify = "verify";
            public const string GetListFiltered = "get-list-filtered";
            public const string Import = "import";
        }

        public static class Auth
        {
            public const string Register = "/api/auth/register";
            public const string Login = "/api/auth/login";
            public const string SendOtp = "/api/auth/send-otp";
            public const string VerifyUser = "/api/auth/verify-user";
            public const string RefreshToken = "/api/auth/refresh-token";
            public const string InternalToken = "/api/auth/internal-token";
            public const string VerifySession = "/api/auth/verify-session";
            public const string GetClaims = "/api/auth/get-claims";
            public const string Logout = "/api/auth/logout";
            public const string ValidateUser = "/api/auth/validate-user";
            public const string GetUserInfo = "/api/users/get-by-id";
            public const string ResetPassword = "/api/auth/reset-password";
        }

        public static class Users
        {
            public const string AppUsersBase = "/api/users";
            public const string SetDefaultApplication = "set-default-application";

            public const string AppRolesBase = "/api/roles";
            public const string UserRoleBase = "/api/user-roles";
            public const string PermissionBase = "/api/permissions";
            public const string AppRolePermissionBase = "/api/role-permissions";
            public const string PermissionTargetBase = "/api/permission-targets";

            public const string AppUserPermissionScopesBase = "/api/users/permission-scopes";

            public const string LookupUsersForRoleAssignment = "lookup-users";


            public const string GetPagesWithRolePermissions = "page-permissions";
            public const string GetPageElementsWithRolePermissions = "page-element-permissions";
        }

		public static class Candidate
		{
			public const string Candidates = "/api/candidates";
			public const string CandidateSkills = "/api/candidate-skills";
			public const string CandidateExperiences = "/api/candidate-experiences";
			public const string CandidateEducations = "/api/candidate-educations";
			public const string CandidateCertifications = "/api/candidate-certifications";
			public const string Resumes = "/api/resumes";
		}
	}
}
