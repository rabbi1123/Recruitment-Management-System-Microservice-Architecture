using Common.Platform.Application.Abstractions;

namespace Candidate.Application.Abstractions.Data
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(string dbKey);
    }
}
