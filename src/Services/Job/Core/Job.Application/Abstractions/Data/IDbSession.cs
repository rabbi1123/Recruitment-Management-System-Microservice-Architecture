using Common.Platform.Application.Abstractions;
using System.Data;

namespace Job.Application.Abstractions.Data
{
    public interface IDbSession
    {
        IDbConnection Connection { get; }
        IDbTransaction? Transaction { get; }
        void Bind(IUnitOfWork uow);
    }
}
