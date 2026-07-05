using Microsoft.Data.SqlClient;

namespace Candidate.WebAPI.Extensions;

public static class ConfigurationExtensions
{
	public static WebApplicationBuilder AddDatabaseConnectionString(this WebApplicationBuilder builder)
	{
		var password = builder.Configuration["Candidate_DB_PASSWORD"];
		if (string.IsNullOrWhiteSpace(password))
		{
			throw new InvalidOperationException(
				"Candidate_DB_PASSWORD is not set. Add it to src/.env, environment variables, or user secrets.");
		}

		var baseConnection = builder.Configuration.GetConnectionString("DefaultConnection")
			?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");

		var resolved = new SqlConnectionStringBuilder(baseConnection)
		{
			Password = password
		}.ConnectionString;

		builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
		{
			["ConnectionStrings:DefaultConnection"] = resolved
		});

		return builder;
	}
}
