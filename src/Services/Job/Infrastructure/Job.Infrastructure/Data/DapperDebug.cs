using Dapper;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Job.Infrastructure.Data
{
	/// <summary>
	/// This class is for only Debugging purposes
	/// </summary>
	internal static class DapperDebug
	{
		/// <summary>
		/// Returns a human-readable SQL string with DynamicParameters values substituted (for debug only).
		/// </summary>
		internal static string RenderSqlWithParameters(string sql, DynamicParameters parameters)
		{
			if (string.IsNullOrEmpty(sql)) return sql;
			if (parameters == null) return sql;

			foreach (var name in parameters.ParameterNames)
			{
				// Dapper stores parameter names without @ normally; format accordingly
				object value;
				try
				{
					value = parameters.Get<object>(name);
				}
				catch
				{
					// some DynamicParameters variants/versions may throw on Get<object>; fall back to null
					value = null;
				}

				var formatted = FormatForSql(value);

				// Regex: match parameter name as whole token (e.g. @Id, not @Identifier)
				var pattern = $@"(?<!\w)@{Regex.Escape(name)}(?!\w)";
				sql = Regex.Replace(sql, pattern, formatted, RegexOptions.CultureInvariant);
			}

			return sql;
		}

		static string FormatForSql(object value)
		{
			if (value == null || value == DBNull.Value) return "NULL";

			// Strings and chars
			if (value is string s)
				return $"'{s.Replace("'", "''")}'";
			if (value is char c)
				return $"'{c.ToString().Replace("'", "''")}'";

			// Date/time
			if (value is DateTime dt)
				return $"'{dt.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)}'";
			if (value is DateTimeOffset dto)
				return $"'{dto.ToString("yyyy-MM-dd HH:mm:ss.fff zzz", CultureInfo.InvariantCulture)}'";

			// Guid
			if (value is Guid g)
				return $"'{g}'";

			// Boolean -> SQL bit
			if (value is bool b)
				return b ? "1" : "0";

			// Byte[] -> hex literal
			if (value is byte[] bytes)
				return "0x" + BitConverter.ToString(bytes).Replace("-", "");

			// IEnumerable (but exclude string/byte[])
			if (value is IEnumerable ie && !(value is string) && !(value is byte[]))
			{
				// flatten enumerable to comma-separated formatted elements
				var items = ie.Cast<object>().Select(FormatForSql).ToArray();
				// If the enumerable is empty, return a SQL empty tuple which will be syntactically safe in IN () contexts use (SELECT 1 WHERE 0=1) is more correct, but for readability:
				if (items.Length == 0) return "(NULL)";
				return "(" + string.Join(", ", items) + ")";
			}

			// Numeric types and others: use invariant ToString
			if (value is IFormattable f)
				return f.ToString(null, CultureInfo.InvariantCulture);

			// Fallback to quoted string
			var txt = value.ToString()?.Replace("'", "''");
			return txt == null ? "NULL" : $"'{txt}'";
		}

		/// <summary>
		/// Add values to the sql with parameters fir debug
		/// </summary>
		internal static void AddValuesToQuery(IDbConnection connection, string query, DynamicParameters parameters)
		{
			var cmd = connection.CreateCommand();
			cmd.CommandText = query;
			foreach (var name in parameters.ParameterNames)
			{
				var param = cmd.CreateParameter();
				param.ParameterName = "@" + name;
				param.Value = parameters.Get<object>(name) ?? DBNull.Value;
				cmd.Parameters.Add(param);
			}
		}
	}
}
