namespace Organization.WebAPI
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

		public static class Organization
		{
			public const string Organizations = "/api/organizations";
			public const string OrganizationMembers = "/api/organization-members";
		}
	}
}
