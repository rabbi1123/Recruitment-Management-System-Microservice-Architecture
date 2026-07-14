using Job.Application.Common;

namespace Job.Infrastructure.Abstractions
{
	public interface IGenericSearchRepository
	{
		Task<(int TotalCount, IEnumerable<T> Records)> ExecuteSearchAsync<T>(GenericSearchQuery request, string baseQuery, Dictionary<string, string>? columnMap = null);
	}
}
