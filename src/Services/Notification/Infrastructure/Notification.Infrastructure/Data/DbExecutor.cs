using Common.DataAccessService.Abstractions;
using Dapper;
using Notification.Application.Abstractions.Data;
using Notification.Infrastructure.Abstractions;

namespace Notification.Infrastructure.Data;

public sealed class DbExecutor : IDbExecutor
{
	private readonly IDbGuard _dbGuard;
	private readonly IDbSession _dbSession;

	public DbExecutor(IDbGuard dbGuard, IDbSession dbSession)
	{
		_dbGuard = dbGuard;
		_dbSession = dbSession;
	}

	public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default)
	{
		_dbGuard.Validate(sql);
		var command = new CommandDefinition(sql, param, _dbSession.Transaction, cancellationToken: cancellationToken);
		return await _dbSession.Connection.QueryAsync<T>(command);
	}

	public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default)
	{
		_dbGuard.Validate(sql);
		var command = new CommandDefinition(sql, param, _dbSession.Transaction, cancellationToken: cancellationToken);
		return await _dbSession.Connection.QueryFirstOrDefaultAsync<T>(command);
	}

	public async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default)
	{
		_dbGuard.Validate(sql);
		var command = new CommandDefinition(sql, param, _dbSession.Transaction, cancellationToken: cancellationToken);
		return await _dbSession.Connection.ExecuteScalarAsync<T>(command);
	}
}
