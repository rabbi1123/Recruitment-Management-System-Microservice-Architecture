using Candidate.Application.Abstractions.Data;
using Candidate.Application.Common.Validation;
using Candidate.Infrastructure.Abstractions;
using Common.Platform.Domain.Abstractions;
using Dapper;
using System.Data;
using System.Reflection;
using System.Text;

namespace Candidate.Infrastructure.Repositories
{
	public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IEntity
	{
		private readonly string _tableName;
		private readonly IDbExecutor _db;

		public GenericRepository(IDbExecutor db)
		{
			_tableName = typeof(TEntity).Name;
			_db = db;
		}

		private static bool IsAllowedType(Type type)
		{
			var underlying = Nullable.GetUnderlyingType(type) ?? type;

			if (underlying.IsPrimitive) return true;

			if (underlying == typeof(string)
				|| underlying == typeof(decimal)
				|| underlying == typeof(DateTime)
				|| underlying == typeof(DateTimeOffset)
				|| underlying == typeof(Guid)
				|| underlying == typeof(TimeSpan)
				|| underlying == typeof(DateOnly)
				|| underlying == typeof(TimeOnly)
				|| underlying == typeof(byte[])
				|| underlying.IsEnum)
				return true;

			// Allow types with custom Dapper TypeHandlers
			if (SqlMapper.HasTypeHandler(underlying))
				return true;

			return false;
		}

		private static DbType? TryMapDbType(Type t)
		{
			var u = Nullable.GetUnderlyingType(t) ?? t;

			if (u == typeof(byte[])) return DbType.Binary;
			if (u == typeof(Guid)) return DbType.Guid;
			if (u == typeof(bool)) return DbType.Boolean;
			if (u == typeof(byte)) return DbType.Byte;
			if (u == typeof(short)) return DbType.Int16;
			if (u == typeof(int)) return DbType.Int32;
			if (u == typeof(long)) return DbType.Int64;
			if (u == typeof(decimal)) return DbType.Decimal;
			if (u == typeof(double)) return DbType.Double;
			if (u == typeof(float)) return DbType.Single;
			if (u == typeof(string)) return DbType.String;
			if (u == typeof(DateTime)) return DbType.DateTime;
			if (u == typeof(DateTimeOffset)) return DbType.DateTimeOffset;
			if (u == typeof(TimeSpan)) return DbType.Time;
			// DateOnly/TimeOnly: assume you registered Dapper TypeHandlers; leave null so handler kicks in.
			return null; // let Dapper infer for the rest
		}

		private static void EnsureCreatedOn(TEntity entity)
		{
			var createdOnProp = typeof(TEntity).GetProperty("CreatedOn");
			if (createdOnProp?.PropertyType != typeof(DateTime))
				return;

			var current = (DateTime)createdOnProp.GetValue(entity)!;
			if (current == default)
				createdOnProp.SetValue(entity, DateTime.UtcNow);
		}

		public async Task<long> AddAsync(TEntity entity)
		{
			ValidationUtils.NormalizeStrings(entity);
			EnsureCreatedOn(entity);

			var pk = entity.GetPrimaryKeyName();

			var properties = typeof(TEntity).GetProperties()
							.Where(p => p.Name != pk
									&& p.Name != "UpdatedOn"
									&& p.Name != "UpdatedBy"
									&& IsAllowedType(p.PropertyType))
							.ToArray();


			var columns = string.Join(", ", properties.Select(p => p.Name));
			var parameters = string.Join(", ", properties.Select(p => "@" + p.Name));

			var sql = $"INSERT INTO {_tableName} ({columns}) VALUES ({parameters}); SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS Id;";


			var result = await _db.ExecuteScalarAsync(sql, entity);

			if (result == null || result == DBNull.Value)
				throw new Exception("Insert failed. No Id returned.");

			return Convert.ToInt64(result);
		}

		public async Task<int> UpdateAsync(TEntity entity)
		{
			ValidationUtils.NormalizeStrings(entity);

			var pk = entity.GetPrimaryKeyName();
			var pkValue = entity.GetPrimaryKeyValue();

			var properties = typeof(TEntity).GetProperties()
							.Where(p => p.Name != pk
									&& p.Name != "CreatedOn"
									&& p.Name != "CreatedBy"
									&& IsAllowedType(p.PropertyType))
								.ToArray();

			var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
			var sql = $"UPDATE {_tableName} SET {setClause} WHERE {pk} = @Id";

			var parameters = new DynamicParameters(entity);
			parameters.Add("Id", pkValue);


			return await _db.ExecuteAsync(sql, parameters);
		}

		public async Task<int> DeleteAsync(long id)
		{
			var entity = Activator.CreateInstance<TEntity>();

			var pk = entity.GetPrimaryKeyName();

			var sql = $"DELETE FROM {_tableName} WHERE {pk} = @Id";
			var parameters = new { Id = id };


			return await _db.ExecuteAsync(sql, parameters);
		}

		public async Task<TEntity> GetByIdAsync(long id)
		{

			var entity = Activator.CreateInstance<TEntity>();
			var pk = entity.GetPrimaryKeyName();

			var columnNames = string.Join(", ",
							 typeof(TEntity).GetProperties()
								 .Where(p => IsAllowedType(p.PropertyType))
								 .Select(p => p.Name));


			var sql = $"SELECT {columnNames} FROM {_tableName} WHERE {pk} = @Id";
			var parameters = new { Id = id };


			var result = await _db.QueryFirstOrDefaultAsync<TEntity>(sql, parameters);

			return result;
		}

		public async Task<List<TEntity>> GetAllAsync(string? sortBy, string? sortOrder)
		{
			var entity = Activator.CreateInstance<TEntity>();

			var columnNames = string.Join(", ",
							 typeof(TEntity).GetProperties()
								 .Where(p => IsAllowedType(p.PropertyType))
								 .Select(p => p.Name));

			var sql = $"SELECT {columnNames} FROM {_tableName}";

			if (string.IsNullOrWhiteSpace(sortBy))
			{
				var hasCreatedOn = typeof(TEntity).GetProperty("CreatedOn") != null;


				if (hasCreatedOn)
				{
					sql += " ORDER BY CreatedOn DESC";
				}
			}
			else
			{
				string orderClause = BuildOrderClause(sortBy, sortOrder);

				sql += orderClause;
			}

			var result = await _db.QueryAsync<TEntity>(sql);

			return result.ToList();
		}

		public async Task<List<TEntity>> GetByPropertiesAsync(object filters, bool useOr = false, bool includeNulls = true)
		{
			if (filters is null) throw new ArgumentNullException(nameof(filters));

			var filterDict = filters.GetType().GetProperties()
								.ToDictionary(p => p.Name, p => p.GetValue(filters));

			if (!filterDict.Any())
				throw new ArgumentException("At least one property must be provided.", nameof(filters));

			// Only allow properties that actually exist on TEntity (avoid typos / injection in identifiers)
			var entityProps = typeof(TEntity).GetProperties()
								.Where(p => IsAllowedType(p.PropertyType))
								.Select(p => p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

			var effectiveFilters = filterDict.Where(kv => entityProps.Contains(kv.Key)).ToList();
			if (effectiveFilters.Count == 0)
				throw new ArgumentException("None of the provided filter properties exist on the entity.", nameof(filters));

			// SELECT column list
			var columnNames = string.Join(", ", entityProps.Select(n => $"{n}"));

			var joiner = useOr ? " OR " : " AND ";
			var conditions = new List<string>();
			var parameters = new DynamicParameters();


			foreach (var (name, value) in effectiveFilters)
			{
				if (value is null)
				{
					if (includeNulls)
					{
						conditions.Add($"{name} IS NULL");
					}

				}
				else
				{
					// Optional: handle IEnumerable for IN (...) if you ever pass arrays/lists
					if (value is System.Collections.IEnumerable seq && value is not string)
					{
						// Convert to list
						var list = new List<object>();
						foreach (var item in seq) list.Add(item!);
						if (list.Count == 0)
						{
							// empty IN should never match; generate "1=0"
							conditions.Add("1 = 0");
						}
						else
						{
							// name_0, name_1, ...
							var placeholders = new List<string>(list.Count);
							for (int i = 0; i < list.Count; i++)
							{
								var pname = $"{name}_{i}";
								placeholders.Add($"@{pname}");
								parameters.Add(pname, list[i]);
							}
							conditions.Add($"{name} IN ({string.Join(", ", placeholders)})");
						}
					}
					else
					{
						conditions.Add($"{name} = @{name}");
						parameters.Add(name, value);
					}
				}
			}

			var whereClause = string.Join(joiner, conditions);

			if (string.IsNullOrWhiteSpace(whereClause))
			{
				return new List<TEntity>();
			}

			var sql = $"SELECT {columnNames} FROM {_tableName} WHERE {whereClause}";


			var result = await _db.QueryAsync<TEntity>(sql, parameters);

			return result.ToList();
		}

		private string BuildOrderClause(string? sortBy, string? sortOrder)
		{
			// Case 1: propertyName provided
			if (!string.IsNullOrWhiteSpace(sortBy))
			{
				string pascal = ToPascalCase(sortBy);

				var prop = typeof(TEntity)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance)
					.FirstOrDefault(p => p.Name == pascal);

				if (prop != null)
				{
					string dir = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase)
						? "DESC"
						: "ASC";

					return $" ORDER BY {prop.Name} {dir}";
				}
			}

			// Case 2: fallback to CreatedOn DESC
			var createdOnProp = typeof(TEntity)
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.FirstOrDefault(p => p.Name == "CreatedOn");

			if (createdOnProp != null)
			{
				return " ORDER BY CreatedOn DESC";
			}

			// Case 3: no valid property
			return string.Empty;
		}

		private static string ToPascalCase(string camel)
		{
			if (string.IsNullOrEmpty(camel)) return camel;
			if (char.IsUpper(camel[0])) return camel; // already PascalCase
			return char.ToUpperInvariant(camel[0]) + camel.Substring(1);
		}


		public async Task<List<long>> BulkInsertAsync(List<TEntity> entities)
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));
			if (entities.Count == 0) return new List<long>();

			foreach (var e in entities)
			{
				ValidationUtils.NormalizeStrings(e);
				EnsureCreatedOn(e);
			}

			var pk = Activator.CreateInstance<TEntity>().GetPrimaryKeyName();
			var props = typeof(TEntity).GetProperties()
				.Where(p => p.Name != pk
							&& p.Name != "UpdatedOn"
							&& p.Name != "UpdatedBy"
							&& IsAllowedType(p.PropertyType))
				.ToArray();

			if (props.Length == 0)
				throw new InvalidOperationException($"No insertable columns for {_tableName}.");

			var columnList = string.Join(", ", props.Select(p => p.Name));

			const int PARAM_LIMIT = 2100;
			int colsPerRow = props.Length;
			int maxRowsPerBatch = Math.Max(1, PARAM_LIMIT / Math.Max(1, colsPerRow) - 1);

			var ids = new List<long>(entities.Count);

			for (int i = 0; i < entities.Count; i += maxRowsPerBatch)
			{
				var batch = entities.Skip(i).Take(maxRowsPerBatch).ToList();
				var sb = new StringBuilder();
				var dp = new DynamicParameters();

				sb.AppendLine("DECLARE @Ids TABLE (Id BIGINT);");
				sb.Append($"INSERT INTO {_tableName} ({columnList})");
				sb.Append($" OUTPUT INSERTED.{pk} INTO @Ids(Id)");
				sb.Append(" VALUES ");

				for (int r = 0; r < batch.Count; r++)
				{
					if (r > 0) sb.Append(", ");
					var row = batch[r];

					var rowParams = new List<string>(colsPerRow);
					foreach (var p in props)
					{
						string pname = $"@r{r}_{p.Name}";
						rowParams.Add(pname);

						var value = p.GetValue(row);
						var dbType = TryMapDbType(p.PropertyType);

						if (dbType.HasValue)
							dp.Add(pname, value, dbType: dbType.Value);
						else
							dp.Add(pname, value); // let Dapper infer / use type handler
					}
					sb.Append("(").Append(string.Join(", ", rowParams)).Append(")");
				}

				sb.AppendLine(";");
				sb.AppendLine("SELECT Id FROM @Ids;");

				var sql = sb.ToString();


				var newIds = (await _db.QueryAsync<long>(sql, dp)).ToList();
				ids.AddRange(newIds);
			}

			return ids;
		}

		public async Task<List<long>> BulkUpdateAsync(List<TEntity> entities)
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));
			if (entities.Count == 0) return new List<long>();

			foreach (var e in entities) ValidationUtils.NormalizeStrings(e);

			var pkName = Activator.CreateInstance<TEntity>().GetPrimaryKeyName();
			var pkProp = typeof(TEntity).GetProperty(pkName)
						 ?? throw new InvalidOperationException($"Primary key '{pkName}' not found on {typeof(TEntity).Name}.");

			var updatable = typeof(TEntity).GetProperties()
				.Where(p => p.Name != pkName
							&& p.Name != "CreatedOn"
							&& p.Name != "CreatedBy"
							&& IsAllowedType(p.PropertyType))
				.ToArray();

			if (updatable.Length == 0)
				throw new InvalidOperationException($"No updatable columns for {_tableName}.");

			// Params per row = updatable cols + 1 (PK)
			const int PARAM_LIMIT = 2100;
			int paramsPerRow = updatable.Length + 1;
			int maxRowsPerBatch = Math.Max(1, PARAM_LIMIT / Math.Max(1, paramsPerRow) - 1);

			var updatedIds = new List<long>(entities.Count);

			for (int i = 0; i < entities.Count; i += maxRowsPerBatch)
			{
				var batch = entities.Skip(i).Take(maxRowsPerBatch).ToList();
				var sb = new StringBuilder();
				var dp = new DynamicParameters();

				sb.AppendLine("DECLARE @Ids TABLE (Id BIGINT);");

				for (int r = 0; r < batch.Count; r++)
				{
					var row = batch[r];
					var pkVal = pkProp.GetValue(row);
					if (pkVal is null)
						throw new ArgumentException("Entity PK cannot be null for update.", nameof(entities));

					var setClause = string.Join(", ", updatable.Select(p => $"{p.Name} = @u{r}_{p.Name}"));
					sb.Append($"UPDATE {_tableName} SET {setClause} WHERE {pkName} = @u{r}_Id;");
					// collect only rows that actually matched/updated
					sb.AppendLine("IF @@ROWCOUNT > 0 INSERT INTO @Ids(Id) VALUES (@u" + r + "_Id);");

					dp.Add($"u{r}_Id", pkVal);

					// Column params with explicit DbType where necessary (byte[] etc.), even when value is null
					foreach (var p in updatable)
					{
						var pname = $"u{r}_{p.Name}";
						var val = p.GetValue(row);
						var dbType = TryMapDbType(p.PropertyType);
						if (dbType.HasValue)
							dp.Add(pname, val, dbType: dbType.Value);
						else
							dp.Add(pname, val);  // let Dapper infer / use your type handlers
					}
				}

				sb.AppendLine("SELECT Id FROM @Ids;");

				var sql = sb.ToString();


				var idsBatch = (await _db.QueryAsync<long>(sql, dp)).ToList();
				updatedIds.AddRange(idsBatch);
			}

			return updatedIds;
		}

		public async Task<int> BulkDeleteAsync(List<long> ids)
		{
			var entity = Activator.CreateInstance<TEntity>();

			var pk = entity.GetPrimaryKeyName();

			var sql = $"DELETE FROM {_tableName} WHERE {pk} IN @Ids";
			var parameters = new { Ids = ids };


			return await _db.ExecuteAsync(sql, parameters);
		}

		public async Task<int> DeleteByKeyAsync(object key, string keyName)
		{
			var entity = Activator.CreateInstance<TEntity>();

			var sql = $"DELETE FROM {_tableName} WHERE {keyName} = @Key";
			var parameters = new { Key = key };


			var result = await _db.ExecuteAsync(
				sql,
				parameters);

			return result;
		}
	}

}
