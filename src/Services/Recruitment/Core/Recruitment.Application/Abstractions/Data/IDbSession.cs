using Common.Platform.Application.Abstractions;
using System.Data;

namespace Recruitment.Application.Abstractions.Data
{
    public interface IDbSession
    {
        IDbConnection Connection { get; }
        IDbTransaction? Transaction { get; }
        void Bind(IUnitOfWork uow);
    }
}
