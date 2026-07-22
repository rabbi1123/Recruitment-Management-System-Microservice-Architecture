using Recruitment.Application.Common;

namespace Recruitment.Infrastructure.Abstractions
{
	public interface IGenericSearchRepository
	{
		Task<(int TotalCount, IEnumerable<T> Records)> ExecuteSearchAsync<T>(GenericSearchQuery request, string baseQuery, Dictionary<string, string>? columnMap = null);
	}
}
