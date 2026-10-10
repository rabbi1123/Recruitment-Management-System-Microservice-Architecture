using Notification.Domain.Entities;

namespace Notification.Application.Interfaces;

public interface INotificationRepository
{
	Task<long> AddAsync(Notifications entity, CancellationToken cancellationToken = default);

	Task<Notifications?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

	Task<List<Notifications>> GetAllAsync(string? sortBy = null, string? sortOrder = null, CancellationToken cancellationToken = default);

	Task<bool> ExistsAsync(Guid eventId, string channel, CancellationToken cancellationToken = default);
}
