using Recruitment.Application.Abstractions.Data;
using Common.DataAccessService.Abstractions;
using Common.DataAccessService.Data;
using Common.Platform.Application.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Recruitment.Infrastructure.Data
{
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
			if (!_dbConnections.ConnectionStrings.TryGetValue(dbKey, out var cs))
				throw new KeyNotFoundException($"Connection '{dbKey}' not configured.");

			return new SqlUnitOfWork(
				cs,
				_loggerFactory.CreateLogger<SqlUnitOfWork>(),
				_dbConnectionFactory);
		}
	}
}
