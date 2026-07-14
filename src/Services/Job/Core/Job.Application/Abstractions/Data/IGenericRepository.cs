using System.Collections.Generic;
using System.Threading.Tasks;

namespace Job.Application.Abstractions.Data
{
    public interface IGenericRepository<TEntity>
    {
        Task<long> AddAsync(TEntity entity);
        Task<int> UpdateAsync(TEntity entity);
        Task<int> DeleteAsync(long id);
        Task<TEntity> GetByIdAsync(long id);
        Task<List<TEntity>> GetAllAsync(string? sortBy = null, string? sortOrder = null);
        Task<List<TEntity>> GetByPropertiesAsync(object filters, bool useOr = false, bool includeNulls = true);


        Task<List<long>> BulkInsertAsync(List<TEntity> entities);
        Task<List<long>> BulkUpdateAsync(List<TEntity> entities);
        Task<int> BulkDeleteAsync(List<long> ids);
        Task<int> DeleteByKeyAsync(object key, string keyName);
    }
}
