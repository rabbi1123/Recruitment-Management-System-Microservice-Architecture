using Notification.Application.Abstractions.Data;
using System.Data;

namespace Notification.Infrastructure.Abstractions;

public interface IDbExecutor
{
	Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default);

	Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default);

	Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default);
}
