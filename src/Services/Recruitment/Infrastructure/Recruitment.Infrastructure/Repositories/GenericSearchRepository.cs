using Recruitment.Application.Common;
using Recruitment.Infrastructure.Abstractions;
using Recruitment.Infrastructure.Data;
using Dapper;

namespace Recruitment.Infrastructure.Repositories
{
	public class GenericSearchRepository : IGenericSearchRepository
	{
		private readonly IDbExecutor _db;

		private const string StandardAlias = "AliasX";

		public GenericSearchRepository(IDbExecutor db)
		{
			_db = db;
		}

		public async Task<(int TotalCount, IEnumerable<T> Records)> ExecuteSearchAsync<T>(
			GenericSearchQuery request,
			string baseQuery,
			Dictionary<string, string>? columnMap = null)
		{
			var parameters = new DynamicParameters();
			int paramIndex = 0;

			bool isCteQuery = baseQuery.TrimStart().StartsWith("WITH", StringComparison.OrdinalIgnoreCase);

			string whereClause = GenericQueryBuilder.BuildWhereClause<T>(request, parameters, columnMap, ref paramIndex);
			string orderClause = GenericQueryBuilder.BuildOrderClause<T>(request.SortBy?.PropertyName, request.SortBy?.SortBy, columnMap);

			string countQuery = GenerateCountQuery(baseQuery, whereClause, isCteQuery);

			string paginatedQuery = GeneratePaginatedQuery(baseQuery, whereClause, orderClause, request, isCteQuery);

			var result = await ExecuteAndMapAsync<T>(countQuery, paginatedQuery, parameters);

			return result;
		}

		public string GenerateCountQuery(string baseQuery, string whereClause, bool isCteQuery)
		{
			if (isCteQuery)
			{
				return $@"
                    {baseQuery.TrimEnd(';')}
                    SELECT COUNT(*) AS TotalCount
                    FROM {StandardAlias}
                    {whereClause};";
			}
			else
			{
				return $@"
                    SELECT COUNT(*) AS TotalCount
                    FROM ({baseQuery.TrimEnd(';')} {whereClause}) AS {StandardAlias};";
			}
		}


		private string GeneratePaginatedQuery(
			string baseQuery,
			string whereClause,
			string orderClause,
			GenericSearchQuery request,
			bool isCteQuery)
		{
			return isCteQuery
				? GenerateCtePagedQuery(baseQuery, whereClause, orderClause, request)
				: GenerateBasePagedQuery(baseQuery, whereClause, orderClause, request);
		}

		public async Task<(int TotalCount, IEnumerable<T> Records)> ExecuteAndMapAsync<T>(
			string countQuery,
			string dataQuery,
			DynamicParameters parameters)
		{

			// Execute count query
			int totalCount = await _db.QueryFirstOrDefaultAsync<int>(countQuery, parameters);


			// Execute paginated query
			var records = await _db.QueryAsync<T>(dataQuery, parameters);

			// Extract total count from the first row
			var result = records.ToList();

			return (totalCount, result);
		}

		private string GenerateCtePagedQuery(string baseQuery, string whereClause, string orderClause, GenericSearchQuery request)
		{
			var pageNo = Math.Max(1, request.PageNo);
			var pageSize = (request.PageSize <= 0) ? 50 : request.PageSize;

			var query = $@"
            {baseQuery.TrimEnd(';')}
            SELECT *
            FROM {StandardAlias}
            {whereClause}
            {orderClause}
            OFFSET {(pageNo - 1) * pageSize} ROWS
            FETCH NEXT {pageSize} ROWS ONLY;";


			return query;
		}

		private string GenerateBasePagedQuery(string baseQuery, string whereClause, string orderClause, GenericSearchQuery request)
		{
			var pageNo = Math.Max(1, request.PageNo);
			var pageSize = (request.PageSize <= 0) ? 50 : request.PageSize;

			var query = $@"
            {baseQuery.TrimEnd(';')}
            {whereClause} 
            {orderClause}
            OFFSET {(pageNo - 1) * pageSize} ROWS
            FETCH NEXT {pageSize} ROWS ONLY;";

			return query;
		}
	}
}
