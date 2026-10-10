namespace Notification.Application.Common;

public sealed record SortOption
{
	public string? PropertyName { get; init; }

	public string SortBy { get; init; } = "asc";
}
