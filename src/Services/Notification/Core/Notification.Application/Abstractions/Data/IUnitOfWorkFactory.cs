using Common.Platform.Application.Abstractions;

namespace Notification.Application.Abstractions.Data;

public interface IUnitOfWorkFactory
{
	IUnitOfWork Create(string dbKey);
}
