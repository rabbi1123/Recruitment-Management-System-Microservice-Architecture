using Job.Application.Abstractions.Data;
using Dapper;
using System.Data;

namespace Job.Infrastructure.Abstractions
{
	public interface IDbExecutor
	{
		// -------------------- Single-type query --------------------
		Task<IEnumerable<TReturn>> QueryAsync<TReturn>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null);


		// -------------------- Multi-mapping: 2 types --------------------
		Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null);

		Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null);

		Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null);

		Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null);

		Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null);

		Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null);


		// -------------------- QueryFirstOrDefault --------------------
		Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null);

		// -------------------- Execute --------------------
		Task<int> ExecuteAsync(string sql, object? param = null, int? commandTimeout = 0, CommandType? commandType = null, IDbSession? dbSession = null);

		// -------------------- ExecuteScalar --------------------
		Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null);

		Task<object?> ExecuteScalarAsync(string sql, object? param = null, int? commandTimeout = null, CommandType? commandType = null, IDbSession? dbSession = null);

		// =========================================================
		// 🔷 QUERY MULTIPLE  — callback delegate pattern
		//
		//    WHY CALLBACK and NOT returning GridReader:
		//    Dapper's GridReader holds a live reference to the open
		//    IDataReader.  Returning it to the caller allows the
		//    connection to be released before all result sets are
		//    consumed, causing:
		//      ObjectDisposedException: "The reader has been disposed;
		//      this can happen after all data has been consumed."
		//
		//    The callback pattern keeps ALL reads inside DbExecutor
		//    while the connection + transaction are guaranteed open,
		//    then disposes the GridReader safely when readCallback exits.
		//
		//    USAGE:
		//    await _db.QueryMultipleAsync(spName, async multi =>
		//    {
		//        result.Main = await multi.ReadFirstOrDefaultAsync<T>();
		//        result.Rows = (await multi.ReadAsync<TRow>()).ToList();
		//    }, parameters, commandType: CommandType.StoredProcedure);
		// =========================================================
		Task QueryMultipleAsync(
			string sql,
			Func<SqlMapper.GridReader, Task> readCallback,
			object? param = null,
			int? commandTimeout = null,
			CommandType? commandType = null,
			IDbSession? dbSession = null);
	}
}