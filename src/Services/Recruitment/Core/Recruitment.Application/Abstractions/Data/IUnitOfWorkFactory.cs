using Common.Platform.Application.Abstractions;

namespace Recruitment.Application.Abstractions.Data
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(string dbKey);
    }
}
