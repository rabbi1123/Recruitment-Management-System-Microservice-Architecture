using Common.Platform.Application.Abstractions;

namespace Job.Application.Abstractions.Data
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(string dbKey);
    }
}
