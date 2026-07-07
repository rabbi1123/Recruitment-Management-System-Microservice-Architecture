using Candidate.Application.Abstractions.Data;
using Candidate.Infrastructure.Abstractions;
using Common.DataAccessService.Abstractions;
using Dapper;
using System.Data;

namespace Candidate.Infrastructure.Data
{
	public sealed class DbExecutor : IDbExecutor
	{
		private readonly IDbGuard _dbGuard;
		private readonly IDbSession _dbSession;

		public DbExecutor(IDbGuard dbGuard, IDbSession dbSession)
		{
			_dbGuard = dbGuard;
			_dbSession = dbSession;
		}

		// -------------------- Single-type query --------------------
		public async Task<IEnumerable<TReturn>> QueryAsync<TReturn>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null)
		{
			var db = dbSession ?? _dbSession;
			_dbGuard.Validate(sql, commandType);
			return await db.Connection.QueryAsync<TReturn>(sql, param, db.Transaction, commandTimeout, commandType);
		}

		// -------------------- Multi-mapping--------------------
		public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null)
		{
			var db = dbSession ?? _dbSession;
			_dbGuard.Validate(sql);
			return await db.Connection.QueryAsync(sql, map, param, db.Transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null)
		{
			var db = dbSession ?? _dbSession;
			_dbGuard.Validate(sql, commandType);
			return await db.Connection.QueryAsync(sql, map, param, db.Transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null)
		{
			var db = dbSession ?? _dbSession;
			_dbGuard.Validate(sql, commandType);
			return await db.Connection.QueryAsync(sql, map, param, db.Transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null)
		{
			var db = dbSession ?? _dbSession;
			_dbGuard.Validate(sql, commandType);
			return await db.Connection.QueryAsync(sql, map, param, db.Transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null)
		{
			var db = dbSession ?? _dbSession;
			_dbGuard.Validate(sql, commandType);
			return await db.Connection.QueryAsync(sql, map, param, db.Transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null)
		{
			var db = dbSession ?? _dbSession;
			_dbGuard.Validate(sql, commandType);
			return await db.Connection.QueryAsync(sql, map, param, db.Transaction, buffered, splitOn, commandTimeout, commandType);
		}



		// -------------------- QueryFirstOrDefault --------------------
		public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null)
		{
			var db = dbSession ?? _dbSession;
			_dbGuard.Validate(sql, commandType);
			return await db.Connection.QueryFirstOrDefaultAsync<T>(sql, param, db.Transaction, commandTimeout, commandType);
		}

		// -------------------- Execute --------------------
		public async Task<int> ExecuteAsync(string sql, object? param = null, int? commandTimeout = 0, CommandType? commandType = null, IDbSession? dbSession = null)
		{
			var db = dbSession ?? _dbSession;
			_dbGuard.Validate(sql, commandType);
			return await db.Connection.ExecuteAsync(sql, param, db.Transaction, commandTimeout, commandType);
		}

		// -------------------- ExecuteScalar --------------------
		public async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null)
		{
			var db = dbSession ?? _dbSession;
			_dbGuard.Validate(sql, commandType);
			return await db.Connection.ExecuteScalarAsync<T>(sql, param, db.Transaction, commandTimeout, commandType);
		}

		public async Task<object?> ExecuteScalarAsync(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null)
		{
			var db = dbSession ?? _dbSession;
			_dbGuard.Validate(sql, commandType);
			return await db.Connection.ExecuteScalarAsync(sql, param, db.Transaction, commandTimeout, commandType);
		}

		// =========================================================
		// 🔷 QUERY MULTIPLE  — callback delegate pattern
		//
		//    The `using` is owned HERE inside DbExecutor, not by the
		//    caller.  The readCallback runs while the GridReader (and
		//    therefore the underlying IDataReader + connection) is
		//    guaranteed open.  Only after readCallback completes does
		//    the GridReader dispose — eliminating ObjectDisposedException.
		// =========================================================
		public async Task QueryMultipleAsync(
			string sql,
			Func<SqlMapper.GridReader, Task> readCallback,
			object? param = null,
			int? commandTimeout = null,
			CommandType? commandType = null,
			IDbSession? dbSession = null)
		{
			ArgumentNullException.ThrowIfNull(readCallback);

			var db = dbSession ?? _dbSession;

			_dbGuard.Validate(sql, commandType);

			using var multi = await db.Connection.QueryMultipleAsync(
				sql: sql,
				param: param,
				transaction: db.Transaction,
				commandTimeout: commandTimeout,
				commandType: commandType
			);

			await readCallback(multi);
		}
	}
}