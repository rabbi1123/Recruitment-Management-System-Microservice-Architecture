using Common.Platform.Application.Abstractions;
using System.Data;

namespace Notification.Application.Abstractions.Data;

public sealed class DbSession : IDbSession
{
	private IUnitOfWork? _unitOfWork;

	public void Bind(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public IDbConnection Connection =>
		_unitOfWork?.Connection
		?? throw new InvalidOperationException("DbSession is not bound.");

	public IDbTransaction? Transaction => _unitOfWork?.Transaction;
}
