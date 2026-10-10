using Notification.Application.Interfaces;
using Notification.Domain.Entities;
using Notification.Infrastructure.Abstractions;

namespace Notification.Infrastructure.Persistence;

public sealed class NotificationRepository : INotificationRepository
{
	private const string Columns = """
		Id, EventId, UserId, Channel, Recipient, TemplateName, TemplateVersion, Title, Subject, Body,
		Status, RetryCount, Provider, ProviderMessageId, ErrorCode, ErrorMessage, ScheduledOn, SentOn,
		DeliveredOn, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy
		""";

	private readonly IDbExecutor _db;

	public NotificationRepository(IDbExecutor db)
	{
		_db = db;
	}

	public async Task<long> AddAsync(Notifications entity, CancellationToken cancellationToken = default)
	{
		const string sql = """
			INSERT INTO Notifications
			(
				EventId, UserId, Channel, Recipient, TemplateName, TemplateVersion, Title, Subject, Body,
				Status, RetryCount, Provider, ProviderMessageId, ErrorCode, ErrorMessage, ScheduledOn, SentOn,
				DeliveredOn, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy
			)
			VALUES
			(
				@EventId, @UserId, @Channel, @Recipient, @TemplateName, @TemplateVersion, @Title, @Subject, @Body,
				@Status, @RetryCount, @Provider, @ProviderMessageId, @ErrorCode, @ErrorMessage, @ScheduledOn, @SentOn,
				@DeliveredOn, @CreatedOn, @CreatedBy, @UpdatedOn, @UpdatedBy
			);
			SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
			""";

		var id = await _db.ExecuteScalarAsync<long>(sql, entity, cancellationToken);
		return id;
	}

	public Task<Notifications?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
	{
		var sql = $"SELECT {Columns} FROM Notifications WHERE Id = @Id";
		return _db.QueryFirstOrDefaultAsync<Notifications>(sql, new { Id = id }, cancellationToken);
	}

	public async Task<List<Notifications>> GetAllAsync(string? sortBy = null, string? sortOrder = null, CancellationToken cancellationToken = default)
	{
		var sql = $"SELECT {Columns} FROM Notifications ORDER BY {OrderBy(sortBy, sortOrder)}";
		var rows = await _db.QueryAsync<Notifications>(sql, cancellationToken: cancellationToken);
		return rows.ToList();
	}

	public async Task<bool> ExistsAsync(Guid eventId, string channel, CancellationToken cancellationToken = default)
	{
		const string sql = """
			SELECT CASE WHEN EXISTS (
				SELECT 1 FROM Notifications WHERE EventId = @EventId AND Channel = @Channel
			) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END
			""";

		var exists = await _db.QueryFirstOrDefaultAsync<bool>(sql, new { EventId = eventId, Channel = channel }, cancellationToken);
		return exists;
	}

	private static string OrderBy(string? sortBy, string? sortOrder)
	{
		var column = sortBy switch
		{
			"Id" => "Id",
			"Channel" => "Channel",
			"Status" => "Status",
			"Recipient" => "Recipient",
			"TemplateName" => "TemplateName",
			"SentOn" => "SentOn",
			"CreatedOn" => "CreatedOn",
			_ => "CreatedOn"
		};

		var direction = string.Equals(sortOrder, "asc", StringComparison.OrdinalIgnoreCase) ? "ASC" : "DESC";
		return $"{column} {direction}";
	}
}
