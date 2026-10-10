using Common.DataAccessService.Abstractions;
using Common.DataAccessService.Data;
using Common.Platform.Application.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notification.Application.Abstractions.Data;

namespace Notification.Infrastructure.Data;

public sealed class SqlUnitOfWorkFactory : IUnitOfWorkFactory
{
	private readonly DbConnections _dbConnections;
	private readonly ILoggerFactory _loggerFactory;
	private readonly IDbConnectionFactory _dbConnectionFactory;

	public SqlUnitOfWorkFactory(
		IOptions<DbConnections> dbConnections,
		ILoggerFactory loggerFactory,
		IDbConnectionFactory dbConnectionFactory)
	{
		_dbConnections = dbConnections.Value;
		_loggerFactory = loggerFactory;
		_dbConnectionFactory = dbConnectionFactory;
	}

	public IUnitOfWork Create(string dbKey)
	{
		if (!_dbConnections.ConnectionStrings.TryGetValue(dbKey, out var connectionString))
			throw new KeyNotFoundException($"Connection '{dbKey}' is not configured.");

		return new SqlUnitOfWork(
			connectionString,
			_loggerFactory.CreateLogger<SqlUnitOfWork>(),
			_dbConnectionFactory);
	}
}
