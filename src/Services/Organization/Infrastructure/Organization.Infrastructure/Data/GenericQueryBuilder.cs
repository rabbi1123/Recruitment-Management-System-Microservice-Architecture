using Organization.Application.Common;
using Common.Platform.Domain.Abstractions;
using Dapper;

namespace Organization.Infrastructure.Data
{
	public static class GenericQueryBuilder
	{
		public static string GeneratePaginatedQuery(string baseQuery, string orderClause, int pageNo, int pageSize)
		{

			pageNo = Math.Max(1, pageNo);
			pageSize = (pageSize <= 0) ? 50 : pageSize;
			long offset = ((long)pageNo - 1) * pageSize;

			var query = $@"
            {baseQuery.TrimEnd(';')}
            {orderClause}
            OFFSET {offset} ROWS
            FETCH NEXT {pageSize} ROWS ONLY";

			return query;
		}

		public static void ValidateFiltersWithColumnMap(List<SearchFilter> filters, Dictionary<string, string> columnMap)
		{
			if (filters != null && filters.Any())
			{
				foreach (var f in filters)
				{
					if (!columnMap.ContainsKey(f.PropertyName) && columnMap.Count != 0)
					{
						throw new ValidationException(Error.BadRequest($"Invalid propertyName:{f.PropertyName} provided"));
					}
				}
			}
		}

		public static List<string> BuildFilterConditions(
			List<SearchFilter> filters,
			DynamicParameters parameters,
			Dictionary<string, string> columnMap,
			ref int paramIndex,
			string? paramPrefix = null)
		{
			var conditions = new List<string>();

			if (filters == null || !filters.Any()) return conditions;

			paramPrefix = string.IsNullOrWhiteSpace(paramPrefix) ? "p" : paramPrefix;

			for (int i = 0; i < filters.Count; i++)
			{
				var f = filters[i];

				string paramName = $"@{paramPrefix}_{f.PropertyName}_p{paramIndex++}";
				string column = string.Empty;
				string condition;


				var values = f.Value?.ToString()?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

				if (columnMap.ContainsKey(f.PropertyName))
				{
					column = columnMap[f.PropertyName];
				}
				else
				{
					continue;
				}

				switch (f.Comparison)
				{
					case ComparisonOperator.Equals:
						condition = $"{column} = {paramName}";
						parameters.Add(paramName, f.Value);
						break;
					case ComparisonOperator.NotEquals:
						condition = $"{column} <> {paramName}";
						parameters.Add(paramName, f.Value);
						break;
					case ComparisonOperator.Contains:
						condition = $"{column} LIKE {paramName}";
						parameters.Add(paramName, $"%{f.Value}%");
						break;
					case ComparisonOperator.NotContains:
						condition = $"{column} NOT LIKE {paramName}";
						parameters.Add(paramName, $"%{f.Value}%");
						break;
					case ComparisonOperator.StartsWith:
						condition = $"{column} LIKE {paramName}";
						parameters.Add(paramName, $"{f.Value}%");
						break;
					case ComparisonOperator.EndsWith:
						condition = $"{column} LIKE {paramName}";
						parameters.Add(paramName, $"%{f.Value}");
						break;
					case ComparisonOperator.IsEmpty:
						condition = $"({column} IS NULL OR {column} = '')";
						break;
					case ComparisonOperator.IsNotEmpty:
						condition = $"({column} IS NOT NULL AND {column} <> '')";
						break;
					case ComparisonOperator.IsNull:
						condition = $"{column} IS NULL";
						break;
					case ComparisonOperator.IsNotNull:
						condition = $"{column} IS NOT NULL";
						break;
					case ComparisonOperator.GreaterThan:
						condition = $"{column} > {paramName}";
						parameters.Add(paramName, f.Value);
						break;
					case ComparisonOperator.GreaterThanOrEqual:
						condition = $"{column} >= {paramName}";
						parameters.Add(paramName, f.Value);
						break;
					case ComparisonOperator.LessThan:
						condition = $"{column} < {paramName}";
						parameters.Add(paramName, f.Value);
						break;
					case ComparisonOperator.LessThanOrEqual:
						condition = $"{column} <= {paramName}";
						parameters.Add(paramName, f.Value);
						break;
					case ComparisonOperator.IsTrue:
						condition = $"{column} = 1";
						break;
					case ComparisonOperator.IsFalse:
						condition = $"{column} = 0";
						break;

					// DATE
					case ComparisonOperator.IsBefore:
						condition = $"{column} < {paramName}";
						parameters.Add(paramName, f.Value);
						break;

					case ComparisonOperator.IsAfter:
						condition = $"{column} > {paramName}";
						parameters.Add(paramName, f.Value);
						break;

					case ComparisonOperator.IsOnOrBefore:
						condition = $"{column} <= {paramName}";
						parameters.Add(paramName, f.Value);
						break;

					case ComparisonOperator.IsOnOrAfter:
						condition = $"{column} >= {paramName}";
						parameters.Add(paramName, f.Value);
						break;

					case ComparisonOperator.IsExactly:
						condition = $"{column} = {paramName}";
						parameters.Add(paramName, f.Value);
						break;

					case ComparisonOperator.IsNotExactly:
						condition = $"{column} <> {paramName}";
						parameters.Add(paramName, f.Value);
						break;

					case ComparisonOperator.IsBetween:
						{
							if (values == null || values.Length != 2)
								throw new ArgumentException($"Between filter requires two values for {f.PropertyName}");

							string paramName2 = $"@{paramPrefix}_{f.PropertyName}_p{paramIndex++}";

							condition = $"{column} BETWEEN {paramName} AND {paramName2}";

							parameters.Add(paramName, values[0]);
							parameters.Add(paramName2, values[1]);
							break;
						}

					// ARRAY / MULTI VALUE
					case ComparisonOperator.IncludesAny:
						{
							if (values == null || values.Length == 0)
								throw new ArgumentException($"IncludesAny requires values for {f.PropertyName}");

							condition = $"{column} IN {paramName}";
							parameters.Add(paramName, values);
							break;
						}

					case ComparisonOperator.NotIncludes:
						{
							if (values == null || values.Length == 0)
								throw new ArgumentException($"NotIncludes requires values for {f.PropertyName}");

							condition = $"{column} NOT IN {paramName}";
							parameters.Add(paramName, values);
							break;
						}

					case ComparisonOperator.IncludesAll:
						{
							if (values == null || values.Length == 0)
								throw new ArgumentException($"IncludesAll requires values for {f.PropertyName}");

							var subConditions = new List<string>();

							foreach (var val in values)
							{
								string pn = $"@{paramPrefix}_{f.PropertyName}_p{paramIndex++}";
								subConditions.Add($"{column} = {pn}");
								parameters.Add(pn, val);
							}

							condition = "(" + string.Join(" AND ", subConditions) + ")";
							break;
						}
					default:
						throw new NotSupportedException($"Unsupported comparison: {f.Comparison}");
				}

				if (i > 0 && f.LogicalOperator == LogicalOperator.Or)
					conditions[^1] = $"({conditions[^1]} OR {condition})";
				else
					conditions.Add(condition);
			}

			return conditions;
		}


		public static string BuildGlobalSearchCondition<T>(
			string globalSearch,
			DynamicParameters parameters,
			Dictionary<string, string> columnMap,
			ref int paramIndex,
			string? paramPrefix = null)
		{
			if (string.IsNullOrWhiteSpace(globalSearch)) return string.Empty;
			if (columnMap == null || columnMap.Count == 0) return string.Empty;
			paramPrefix = string.IsNullOrWhiteSpace(paramPrefix) ? "p" : paramPrefix;
			var condition = string.Empty;

			var stringProps = typeof(T).GetProperties()
					.Where(p => p.PropertyType == typeof(string) && columnMap.ContainsKey(char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1)))
					.Select(p => char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1))
					.ToList();

			if (!stringProps.Any()) return string.Empty;

			var globalConditions = new List<string>(stringProps.Count);
			for (int i = 0; i < stringProps.Count; i++)
			{
				var prop = stringProps[i];
				// create unique param name using the ref counter
				var paramName = $"@{paramPrefix}_global_{paramIndex++}";
				parameters.Add(paramName, $"%{globalSearch}%");

				var column = columnMap.ContainsKey(prop) ? columnMap[prop] : prop;
				globalConditions.Add($"{column} LIKE {paramName}");
			}

			condition = $"({string.Join(" OR ", globalConditions)})";

			return condition;
		}

		public static string BuildWhereClause<T>(
			GenericSearchQuery query,
			DynamicParameters parameters,
			Dictionary<string, string> columnMap,
			ref int paramIndex,
			string? paramPrefix = null)
		{
			var conditions = new List<string>();
			columnMap ??= new Dictionary<string, string>();

			// Global search across string properties of T
			var globalConditon = BuildGlobalSearchCondition<T>(query.GlobalSearch, parameters, columnMap, ref paramIndex, paramPrefix);

			if (!string.IsNullOrWhiteSpace(globalConditon))
			{
				conditions.Add(globalConditon);
			}

			// Structured filters
			var newConditions = BuildFilterConditions(query.Search, parameters, columnMap, ref paramIndex, paramPrefix);

			conditions.AddRange(newConditions);

			var finalWhere = "";

			finalWhere = conditions.Any() ?
								 "WHERE " + string.Join(" AND ", conditions)
								 : "";

			return finalWhere;
		}

		public static string BuildOrderClause<T>(string? sortBy, string? sorOrder, Dictionary<string, string>? columnMap = null)
		{
			string column;
			string direction;

			columnMap ??= new Dictionary<string, string>();

			string createdOnColumn = $"createdOn";


			if (!string.IsNullOrWhiteSpace(sortBy))
			{
				column = sortBy;
				direction = string.Equals(sorOrder, "DESC", StringComparison.OrdinalIgnoreCase)
					? "DESC"
					: "ASC";

				if (columnMap.ContainsKey(sortBy))
				{
					column = columnMap[sortBy];
				}
				else if (columnMap.Count != 0)
				{
					throw new ValidationException(Error.BadRequest($"Invalid propertyName:{sortBy} provided for sortBy"));
				}

			}
			else if (columnMap.ContainsKey("createdOn"))
			{
				column = columnMap["createdOn"];
				direction = "DESC";
			}
			else
			{
				// Pick a default property: try numeric first, else first property
				var props = typeof(T).GetProperties();

				var numericProp = props.FirstOrDefault(p =>
					p.PropertyType == typeof(int) ||
					p.PropertyType == typeof(long) ||
					p.PropertyType == typeof(int?) ||
					p.PropertyType == typeof(long?));

				column = $"{numericProp?.Name ?? props.First().Name}";
				direction = "ASC";
			}

			return $" ORDER BY {column} {direction}";
		}
	}
}
