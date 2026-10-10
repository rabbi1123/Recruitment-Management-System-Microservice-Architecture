using Common.Platform.Application.Abstractions;
using System.Data;

namespace Notification.Application.Abstractions.Data;

public interface IDbSession
{
	IDbConnection Connection { get; }

	IDbTransaction? Transaction { get; }

	void Bind(IUnitOfWork unitOfWork);
}
