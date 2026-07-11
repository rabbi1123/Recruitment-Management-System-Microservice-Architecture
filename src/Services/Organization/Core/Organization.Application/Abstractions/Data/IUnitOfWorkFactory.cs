using Common.Platform.Application.Abstractions;

namespace Organization.Application.Abstractions.Data
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(string dbKey);
    }
}
