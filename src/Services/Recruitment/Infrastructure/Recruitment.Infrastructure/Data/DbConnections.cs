namespace Recruitment.Infrastructure.Data
{
	public sealed class DbConnections
	{
		public Dictionary<string, string> ConnectionStrings { get; init; } = new();
	}
}
